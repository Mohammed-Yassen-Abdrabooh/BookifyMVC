using Hangfire;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixLabors.ImageSharp;

namespace Bookify.Web.Controllers
{
    [Authorize(Roles =AppRoles.Reception)]
    public class SubscriberController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IDataProtector _dataProtector;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IWhatsAppClient _whatsAppClient;
        private readonly IImageService _imageService;
        private readonly IEmailBodyBuilder _emailBodyBuilder;
        private readonly IEmailSender _emailSender;
        private List<string> _allowedExtensions = new() { ".jpg", ".jpeg", ".png", ".gif" };
        private int _maxFileSize = 2097152; // 2 MB = 2 * 1024 * 1024;

        public SubscriberController(ApplicationDbContext dbContext, IDataProtectionProvider dataProtector, IMapper mapper, IWebHostEnvironment webHostEnvironment, IWhatsAppClient whatsAppClient, IImageService imageService, IEmailBodyBuilder emailBodyBuilder, IEmailSender emailSender)
        {
            _dbContext = dbContext;
            _dataProtector = dataProtector.CreateProtector("MySecureKey");
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
            _whatsAppClient = whatsAppClient;
            _imageService = imageService;
            _emailBodyBuilder = emailBodyBuilder;
            _emailSender = emailSender;
        }

        #region Index,Search,Details Actions
        public IActionResult Index()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Search(SearchFormViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var subscriber = _dbContext.Subscribers.SingleOrDefault(S => S.MobileNumber == model.SearchValue || S.NationalId == model.SearchValue || S.Email == model.SearchValue);
            var viewModel = _mapper.Map<SubscriberSearchResultViewModel>(subscriber);

            if(subscriber is not null)
                viewModel.Key = _dataProtector.Protect(subscriber.Id.ToString());

            return PartialView("_Result",viewModel);
        }

        public IActionResult Details(string id)
        {
            var subscriberId = int.Parse(_dataProtector.Unprotect(id));
            var subscriber = _dbContext.Subscribers.Include(s => s.Area)
                                                   .Include(s => s.Governorate)
                                                   .Include(s => s.Subscriptions)
                                                   .Include(S => S.Rentals)
                                                   .ThenInclude(r => r.RentalCopies)
                                                   .SingleOrDefault(s => s.Id == subscriberId);

            if (subscriber is null)
                return NotFound();

            var viewModel = _mapper.Map<SubscriberDetailsViewModel>(subscriber);
            viewModel.Key = id;
            return View(viewModel);
        }
        #endregion

        #region Create Get and Post
        [HttpGet]
        public IActionResult Create()
        {
            // Add Governorates Values to Select List Items at ViewModel When Load the Form First Time
            var viewModel = PopulateViewModel();

            return View("Form", viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubscriberFormViewModel model)
        {

            if (!ModelState.IsValid)
            {
                var viewModel = PopulateViewModel(model);

                return View("Form", viewModel);
            }
            var subscriber = _mapper.Map<Subscriber>(model);

            var extension = Path.GetExtension(model.Image!.FileName);
            var imageName = $"{Guid.NewGuid()}{extension}";

            var result = await _imageService.UploadImageAsync(model.Image, imageName, "/images/subscribers", hasThumbnail: true);
            if (!result.IsUploaded)
            {
                ModelState.AddModelError(nameof(Image), result.ErrorMessage!);
                return View("Form", PopulateViewModel(model));
            }

            subscriber.ImageUrl = $"/images/subscribers/{imageName}";
            subscriber.ImageThumbnailUrl = $"/images/subscribers/thumb/{imageName}";
            subscriber.CreatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            Subscription subscription = new()
            {
                CreatedById = subscriber.CreatedById,
                CreatedOn = subscriber.CreatedOn,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddYears(1)
            };

            subscriber.Subscriptions.Add(subscription);

            _dbContext.Subscribers.Add(subscriber);
            _dbContext.SaveChanges();

            //Send Welcome Email to Subscriber

            var placeholders = new Dictionary<string, string>()
            {
                { "imageUrl" , "https://res.cloudinary.com/yassen-bookify/image/upload/v1761162282/icon-positive-vote-2_ts4kvu.jpeg" },
                { "header" , $"Hello {model.FirstName} {model.LastName}" },
                { "body" , "Thanks for joining Bookify 🤩🤩" }
            };

            var body = _emailBodyBuilder.GetEmailBody(MailTemplates.Notification, placeholders);
            var email = _webHostEnvironment.IsDevelopment() ? "mohammedyassen.pc@gmail.com" : model.Email;

            // Use Hangfire to Send Email in Background Job
            BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(email, "Welcome To Bookify", body));


            //Send Welcome Message to Subscriber using WhatsApp Cloud Api
            if (model.HasWhatsApp)
            {
                // use New Created Template with Parameter in Meta WhatsApp Cloud Api and How Send Variables in it, While using "WhatsAppApiClient" Package by Elhelaly
                var components = new List<WhatsAppComponent>()
                {
                    new WhatsAppComponent
                    {
                        Type = "body",
                        Parameters = new List<object>()
                        {
                            new WhatsAppTextParameter { Text = $"{model.FirstName} {model.LastName}"},
                        }
                    }
                };

                var mobileNumber = _webHostEnvironment.IsDevelopment() ? "01094046114" : model.MobileNumber;
                // This Not Work because WhatsApp Message Template Not Approved as Utility but Approved as Marketing so we must use "bookify_welcome_message" Template
                // Then We Use The Default Template "hello_world" from Meta for Testing Purpose
                // Now After Adding "message_bookify","bookify_subscriber_message" Which Created by ChatGPT and Approved as Utility Template We Can Use it Here
                // Use Hangfire to Send WhatsApp Message in Background Job
                BackgroundJob.Enqueue(() => _whatsAppClient.SendMessage($"2{mobileNumber}",
                                                        WhatsAppLanguageCode.English_US,
                                                        WhatsAppTemplates.BookifyMessage, components));

            }
            var subscriberId = _dataProtector.Protect(subscriber.Id.ToString());
            return RedirectToAction(nameof(Details), new { id = subscriberId });
        }
        #endregion

        #region Edit Get and Post
        public IActionResult Edit(string id)
        {
            var subscriberId = int.Parse(_dataProtector.Unprotect(id));
            //var subscriber = _dbContext.Subscribers.Include(s => s.Area).FirstOrDefault(s => s.Id == subscriberId);
            var subscriber = _dbContext.Subscribers.Find(subscriberId);
            if (subscriber is null)
                return NotFound();

            var model = _mapper.Map<SubscriberFormViewModel>(subscriber);
            //PopulateViewModel(model);=> Add Governorates and Areas Values to Select List Items at ViewModel and Return All ViewModel with its All Data
            var subscriberFormViewModel = PopulateViewModel(model);
            subscriberFormViewModel.Key = id;

            return View("Form", subscriberFormViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SubscriberFormViewModel model)
        {

            if (!ModelState.IsValid)
            {
                model = PopulateViewModel(model);
                return View("Form", model);
            }

            var subscriberId = int.Parse(_dataProtector.Unprotect(model.Key!));

            var subscriber = _dbContext.Subscribers.Find(subscriberId);
            if (subscriber is null)
                return NotFound();

            if (model.Image is not null)
            {
                if (!string.IsNullOrEmpty(subscriber.ImageUrl))
                {
                    _imageService.DeleteImage(subscriber.ImageUrl, subscriber.ImageThumbnailUrl);
                }

                var extension = Path.GetExtension(model.Image.FileName);
                var imageName = $"{Guid.NewGuid()}{extension}";

                var result = await _imageService.UploadImageAsync(model.Image, imageName, "/images/subscribers", hasThumbnail: true);
                if (!result.IsUploaded)
                {
                    ModelState.AddModelError(nameof(Image), result.ErrorMessage!);
                    return View("Form", PopulateViewModel());
                }

                model.ImageUrl = $"/images/subscribers/{imageName}";
                model.ImageThumbnailUrl = $"/images/subscribers/thumb/{imageName}";

            }
            else if (model.Image is null && !string.IsNullOrEmpty(subscriber.ImageUrl))
            {
                model.ImageUrl = subscriber.ImageUrl; // Keep the old image if no new image is uploaded
                model.ImageThumbnailUrl = subscriber.ImageThumbnailUrl; // Keep the old Thumbnailimage if no new image is uploaded
            }

            subscriber = _mapper.Map(model, subscriber); // Update the book properties from the model
            subscriber.LastUpdateById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            subscriber.LastUpdateOn = DateTime.Now; // Update the LastUpdateOn property


            _dbContext.SaveChanges();
            return RedirectToAction(nameof(Index), new { id = model.Key });
        }
        #endregion

        #region RenewSubscription Action
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RenewSubscription(string sKey)
        {
            var subscriberId = int.Parse(_dataProtector.Unprotect(sKey));

            var subscriber = _dbContext.Subscribers.Include(s => s.Subscriptions).SingleOrDefault(s => s.Id == subscriberId);
            if (subscriber is null)
                return NotFound();

            if (subscriber.IsBlackListed)
                return BadRequest("Cannot Renew Subscription for BlackListed Subscriber.");

            var lastSubscription = subscriber.Subscriptions.Last();

            var startDate = lastSubscription.EndDate < DateTime.Today ? DateTime.Today : lastSubscription.EndDate.AddDays(1);

            Subscription newSubscription = new()
            {
                CreatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value,
                CreatedOn = DateTime.Now,
                StartDate = startDate,
                EndDate = startDate.AddYears(1)
            };

            subscriber.Subscriptions.Add(newSubscription);
            _dbContext.SaveChanges();

            // TODO : Send Email and WhatsApp Message to Subscriber to Notify Him about Subscription Renewal

            //Send Welcome Email to Subscriber

            var placeholders = new Dictionary<string, string>()
            {
                { "imageUrl" , "https://res.cloudinary.com/yassen-bookify/image/upload/v1761162282/icon-positive-vote-2_ts4kvu.jpeg" },
                { "header" , $"Hello {subscriber.FirstName} {subscriber.LastName}" },
                { "body" , $"Your subscription renewal has been updated. Your subscription ends on {newSubscription.EndDate.ToString("dd MMM, yyyy")}" }
            };

            var body = _emailBodyBuilder.GetEmailBody(MailTemplates.Notification, placeholders);
            var email = _webHostEnvironment.IsDevelopment() ? "mohammedyassen.pc@gmail.com" : subscriber.Email;
            // Use Hangfire to Send Email in Background Job
            BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(email, "Renew Subscription for Bookify", body));

            // Use Hangfire to Schedule Email in Background Job after 1 Minute
            //BackgroundJob.Schedule(() => _emailSender.SendEmailAsync(email, "Renew Subscription for Bookify", body),TimeSpan.FromMinutes(1));

            //Send Welcome Message to Subscriber using WhatsApp Cloud Api
            if (subscriber.HasWhatsApp)
            {
                // use New Created Template with Parameter in Meta WhatsApp Cloud Api and How Send Variables in it, While using "WhatsAppApiClient" Package by Elhelaly
                //var components = new List<WhatsAppComponent>()
                //{
                //    new WhatsAppComponent
                //    {
                //        Type = "body",
                //        Parameters = new List<object>()
                //        {
                //            new WhatsAppTextParameter { Text = $"{subscriber.FirstName} {subscriber.LastName}"},
                //            new WhatsAppTextParameter { Text = newSubscription.EndDate.ToString("dd MMM, yyyy")},
                //        }
                //    }
                //};

                var mobileNumber = _webHostEnvironment.IsDevelopment() ? "01094046114" : subscriber.MobileNumber;
                // Use Hangfire to Send WhatsApp Message in
                BackgroundJob.Enqueue(() => _whatsAppClient.SendMessage($"2{mobileNumber}", WhatsAppLanguageCode.English, WhatsAppTemplates.BookifyRenewSubscription, null));

            }

            var viewModel = _mapper.Map<SubscriptionViewModel>(newSubscription);

            return PartialView("_SubscriptionRow", viewModel);
        }
        #endregion

        #region GetAreas for Government
        [AjaxOnly]
        public IActionResult GetAreas(int governorateId)
        {
            var areas = _dbContext.Areas.Where(a => a.GovernorateId == governorateId && !a.IsDeleted)
                .Select(a => new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = a.Name
                })
                .OrderBy(a => a.Text)
                .ToList();
            return Ok(areas);
        }
        #endregion

        #region Validations Action For SubscriberFormViewModel To Not Add Dupplicate Values To NationalId,MobileNumber,Email
        public IActionResult AllowNationalId(SubscriberFormViewModel model)
        {
            var subscriberId = 0;
            if (!string.IsNullOrEmpty(model.Key))
                subscriberId = int.Parse(_dataProtector.Unprotect(model.Key));


            var subscriber = _dbContext.Subscribers.SingleOrDefault(S => S.NationalId == model.NationalId);
            var isAllowed = subscriber is null || subscriber.Id.Equals(subscriberId);

            return Json(isAllowed); // Return true if the category name does not exist, false otherwise
        }
        public IActionResult AllowMobileNumber(SubscriberFormViewModel model)
        {
            var subscriberId = 0;
            if (!string.IsNullOrEmpty(model.Key))
                subscriberId = int.Parse(_dataProtector.Unprotect(model.Key));

            var subscriber = _dbContext.Subscribers.SingleOrDefault(S => S.MobileNumber == model.MobileNumber);
            var isAllowed = subscriber is null || subscriber.Id.Equals(subscriberId);

            return Json(isAllowed); // Return true if the category name does not exist, false otherwise
        }
        public IActionResult AllowEmail(SubscriberFormViewModel model)
        {
            var subscriberId = 0;
            if (!string.IsNullOrEmpty(model.Key))
                subscriberId = int.Parse(_dataProtector.Unprotect(model.Key));

            var subscriber = _dbContext.Subscribers.SingleOrDefault(S => S.Email == model.Email);
            var isAllowed = subscriber is null || subscriber.Id.Equals(subscriberId);

            return Json(isAllowed); // Return true if the category name does not exist, false otherwise
        }
        #endregion




        private SubscriberFormViewModel PopulateViewModel(SubscriberFormViewModel? model = null)
        {
            var subFormViewModel = (model is null) ? new SubscriberFormViewModel() : model;
            var governorates = _dbContext.Governorates.Where(c => !c.IsDeleted).OrderBy(c => c.Name).ToList();
            subFormViewModel.Governorates = _mapper.Map<IEnumerable<SelectListItem>>(governorates);

            if (model?.GovernorateId > 0)
            {
                var areas = _dbContext.Areas
                    .Where(a => a.GovernorateId == model.GovernorateId && !a.IsDeleted)
                    .OrderBy(a => a.Name)
                    .ToList();
                subFormViewModel.Areas = _mapper.Map<IEnumerable<SelectListItem>>(areas);
            }

            return subFormViewModel;
        }
    }
}

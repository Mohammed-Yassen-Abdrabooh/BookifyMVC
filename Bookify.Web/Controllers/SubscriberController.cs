using Bookify.Web.Core.Models;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection;

namespace Bookify.Web.Controllers
{
    [Authorize(Roles =AppRoles.Reception)]
    public class SubscriberController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IDataProtector _dataProtector;
        private readonly IMapper _mapper;
        private readonly IImageService _imageService;
        private List<string> _allowedExtensions = new() { ".jpg", ".jpeg", ".png", ".gif" };
        private int _maxFileSize = 2097152; // 2 MB = 2 * 1024 * 1024;

        public SubscriberController(ApplicationDbContext dbContext, IDataProtectionProvider dataProtector, IMapper mapper, IImageService imageService)
        {
            _dbContext = dbContext;
            _dataProtector = dataProtector.CreateProtector("MySecureKey");
            _mapper = mapper;
            _imageService = imageService;
        }
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
            var subscriber = _dbContext.Subscribers.Include(s => s.Area).Include(s => s.Governorate).SingleOrDefault(s => s.Id == subscriberId);

            if (subscriber is null)
                return NotFound();

            var viewModel = _mapper.Map<SubscriberDetailsViewModel>(subscriber);
            viewModel.Key = id;
            return View(viewModel);
        }

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
            _dbContext.Subscribers.Add(subscriber);
            _dbContext.SaveChanges();

            //TODO: Send Welcome Email to Subscriber
            var subscriberId = _dataProtector.Protect(subscriber.Id.ToString());
            return RedirectToAction(nameof(Details), new { id = subscriberId });
        }

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
            return RedirectToAction(nameof(Index), new { id = model.Key  });
        }

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
        public IActionResult AllowNationalId(SubscriberFormViewModel model)
        {
            var subscriberId = 0;
            if(!string.IsNullOrEmpty(model.Key))
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

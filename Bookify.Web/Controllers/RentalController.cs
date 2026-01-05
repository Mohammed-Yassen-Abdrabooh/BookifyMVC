using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;

namespace Bookify.Web.Controllers
{
    [Authorize(Roles = AppRoles.Reception)]
    public class RentalController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IDataProtector _dataProtector;
        private readonly IMapper _mapper;

        public RentalController(ApplicationDbContext dbContext, IDataProtectionProvider dataProtector, IMapper mapper)
        {
            _dbContext = dbContext;
            _dataProtector = dataProtector.CreateProtector("MySecureKey");
            _mapper = mapper;
        }

        public IActionResult Create(string sKey)
        {
            var subscriberId = int.Parse(_dataProtector.Unprotect(sKey));
            var subscriber = _dbContext.Subscribers.Include(s=>s.Subscriptions)
                                                   .Include(s=>s.Rentals)
                                                   .ThenInclude(r=>r.RentalCopies)
                                                   .SingleOrDefault(s=> s.Id == subscriberId);

            if( subscriber is null)
                return NotFound();

            var (errorMessage, maxAllowedCopies) =ValidateSubscriber(subscriber);

            if (!string.IsNullOrEmpty(errorMessage))
                return View("NotAllowedRental", errorMessage);

            var viewModel = new RentalFormViewModel
            {
                SubscriberKey = sKey,
                MaxAllowedCopies = maxAllowedCopies

            };

            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(RentalFormViewModel model)
        {
            if(!ModelState.IsValid)
                return View(model);

            var subscriberId = int.Parse(_dataProtector.Unprotect(model.SubscriberKey));
            var subscriber = _dbContext.Subscribers.Include(s => s.Subscriptions)
                                                   .Include(s => s.Rentals)
                                                   .ThenInclude(r => r.RentalCopies)
                                                   .SingleOrDefault(s => s.Id == subscriberId);

            if (subscriber is null)
                return NotFound();

            var (errorMessage, maxAllowedCopies) = ValidateSubscriber(subscriber);

            if (!string.IsNullOrEmpty(errorMessage))
                return View("NotAllowedRental", errorMessage);

            var selectedCopies = _dbContext.BookCopies
                                           .Include( c => c.Book)
                                           .Include( c => c.Rentals)
                                           .Where( c => model.SelectedCopies.Contains(c.SerialNumber))
                                           .ToList();

            var currentSubscriberRentals = _dbContext.Rentals
                                                     .Include(r => r.RentalCopies)
                                                     .ThenInclude(rc => rc.BookCopy)
                                                     .Where(r => r.SubscriberId == subscriberId)
                                                     .SelectMany(r => r.RentalCopies)
                                                     .Where(c => !c.ReturnDate.HasValue)
                                                     .Select(c => c.BookCopy!.BookId)
                                                     .ToList();

            List<RentalCopy> Copies = new();

            foreach (var copy in selectedCopies)
            {
                if (!copy.IsAvailableForRental || !copy.Book!.IsAvailableForRental)
                    return View("NotAllowedRental", Errors.NotAvailableRental);

                if(copy.Rentals.Any(c => !c.ReturnDate.HasValue))
                    return View("NotAllowedRental", Errors.CopyIsInRental);

                if(currentSubscriberRentals.Any(bookId => bookId == copy.BookId))
                    return View("NotAllowedRental", $"This Subscriber already has a copy for '{copy.Book.Title}' book.");

                Copies.Add(new RentalCopy { BookCopyId = copy.Id });
            }

            Rental rental = new()
            {
                RentalCopies = Copies,
                CreatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value,
            };

            subscriber.Rentals.Add(rental);
            _dbContext.SaveChanges();
            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GetCopyDetails(SearchFormViewModel model)
        {
            if(!ModelState.IsValid)
                return BadRequest();

            var copy = _dbContext.BookCopies.Include( c => c.Book )
                                           .SingleOrDefault( c => c.SerialNumber.ToString() == model.SearchValue && !c.IsDeleted && !c.Book!.IsDeleted);
            if(copy is null)
                return NotFound(Errors.InvalidSerialNumber);

            if (!copy.IsAvailableForRental || !copy.Book!.IsAvailableForRental)
                return BadRequest(Errors.NotAvailableRental);

            //Check that copy is not currently rented in an active rental with no return date with subscriber.
            var copyInRental = _dbContext.RentalCopies.Any(c => c.BookCopyId == copy.Id && !c.ReturnDate.HasValue);
            if (copyInRental)
                return BadRequest(Errors.CopyIsInRental);

            var bookCopyViewModel = _mapper.Map<BookCopyViewModel>(copy);

            return PartialView("_CopyDetails",bookCopyViewModel);
        }

        private (string errorMessage , int? maxAllowedCopies) ValidateSubscriber (Subscriber subscriber)
        {
            if (subscriber.IsBlackListed)
                return (errorMessage: Errors.BlackListedSubscriber, maxAllowedCopies: null);

            if (subscriber.Subscriptions.Last().EndDate < DateTime.Today.AddDays((int)RentalsConfiguration.RentalDurationInDays))
                return (errorMessage: Errors.InActiveSubscriber, maxAllowedCopies: null);

            var currentRentalsCount = subscriber.Rentals.SelectMany(r => r.RentalCopies).Count(rc => !rc.ReturnDate.HasValue);

            var availableCopiesAllowed = (int)RentalsConfiguration.MaxNumberOfCopiesPerRental - currentRentalsCount;

            if (availableCopiesAllowed.Equals(0))
                return (errorMessage: Errors.MaxCopiesReached, maxAllowedCopies: null);

            return (errorMessage: string.Empty, maxAllowedCopies: availableCopiesAllowed);
        }
    }
}

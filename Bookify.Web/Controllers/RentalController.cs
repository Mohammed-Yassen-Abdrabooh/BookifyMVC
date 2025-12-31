using Microsoft.AspNetCore.Mvc;

namespace Bookify.Web.Controllers
{
    [Authorize(Roles = AppRoles.Reception)]
    public class RentalController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public RentalController(ApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public IActionResult Create(string sKey)
        {
            var viewModel = new RentalFormViewModel
            {
                SubscriberKey = sKey
            };

            return View(viewModel);
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

            // TODO: Check that copy is not currently rented in an active rental with no return date with subscriber.
            var bookCopyViewModel = _mapper.Map<BookCopyViewModel>(copy);

            return PartialView("_CopyDetails",bookCopyViewModel);
        }
    }
}

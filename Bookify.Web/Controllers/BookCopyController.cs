
namespace Bookify.Web.Controllers
{
    [Authorize(Roles = AppRoles.Archive)]
    public class BookCopyController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public BookCopyController(ApplicationDbContext dbContext,IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        public IActionResult Index()
        {
            return View();
        }

        [AjaxOnly]
        public IActionResult Create(int bookId)
        {
            var book = _dbContext.Books.Find(bookId);

            if (book is null)
                return NotFound();

            var viewModel = new BookCopyFormViewModel()
            {
                BookId = bookId,
                ShowRentalInput = book.IsAvailableForRental
            };

            return PartialView("Form",viewModel);
        }

        [HttpPost]
        public IActionResult Create(BookCopyFormViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var book = _dbContext.Books.Find(model.BookId);

            if (book is null)
                return NotFound();

            var copy = new BookCopy()
            {
                EditionNumber = model.EditionNumber,
                IsAvailableForRental = book.IsAvailableForRental? model.IsAvailableForRental : false,
                CreatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value,
                CreatedOn = DateTime.Now,
            };

            book.Copies.Add(copy);

            _dbContext.Books.Update(book);
            _dbContext.SaveChanges();

            // This step Go To Refresh the Book Copies Table in Book Details View 
            // When You add New Copy Was Not Appeared Until You Refresh the Page Then Now It Will Appear Without Refresh
            // Then We Need a Function to Add or update Row in Table this Fucntion Will Call after Submit the Form and We Was Use (onModelSuccess()) 
            // but This Function Work With DataTable Library NOOOOOOOW We Go To Details View and Add The New Function To Add or Update Row in Table
            var bookCopyViewModel = _mapper.Map<BookCopyViewModel>(copy);
            return PartialView("_BookCopyRow", bookCopyViewModel);

        }

        [AjaxOnly]
        public IActionResult Edit(int id)
        {
            var copy = _dbContext.BookCopies.Include(c=>c.Book).FirstOrDefault(c=>c.Id == id);
            if (copy is null)
                return NotFound();
            var copyViewModel = _mapper.Map<BookCopyFormViewModel>(copy);
            copyViewModel.ShowRentalInput = copy.Book!.IsAvailableForRental;
            return PartialView("Form",copyViewModel);
        }

        [HttpPost]
        public IActionResult Edit(BookCopyFormViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var copy = _dbContext.BookCopies.Include(c=>c.Book).FirstOrDefault(c=>c.Id == model.Id);
            if (copy is null)
                return NotFound();

            copy.EditionNumber = model.EditionNumber;
            copy.IsAvailableForRental = copy.Book!.IsAvailableForRental ? model.IsAvailableForRental : false;
            copy.LastUpdateById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            copy.LastUpdateOn = DateTime.Now;

            _dbContext.BookCopies.Update(copy);
            _dbContext.SaveChanges();

            var bookCopyViewModel = _mapper.Map<BookCopyViewModel>(copy);
            return PartialView("_BookCopyRow", bookCopyViewModel);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleStatus(int id)
        {

            var copy = _dbContext.BookCopies.Find(id);
            if (copy is null)
                return NotFound();

            copy.IsDeleted = !copy.IsDeleted;// Toggle the IsDeleted status like if condition
            copy.LastUpdateById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            copy.LastUpdateOn = DateTime.Now;

            _dbContext.BookCopies.Update(copy);
            _dbContext.SaveChanges();
            return Ok();

        }
    }
}

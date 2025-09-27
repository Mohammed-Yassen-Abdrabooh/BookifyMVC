using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Web.Controllers
{
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
            };

            book.Copies.Add(copy);
            _dbContext.Books.Update(book);
            _dbContext.SaveChanges();

            return Ok();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleStatus(int id)
        {

            var copy = _dbContext.BookCopies.Find(id);
            if (copy is null)
                return NotFound();

            copy.IsDeleted = !copy.IsDeleted;// Toggle the IsDeleted status like if condition
            copy.LastUpdateOn = DateTime.Now;

            _dbContext.BookCopies.Update(copy);
            _dbContext.SaveChanges();
            return Ok();

        }
    }
}

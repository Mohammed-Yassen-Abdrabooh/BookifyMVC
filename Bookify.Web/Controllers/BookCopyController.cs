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

using Bookify.Web.Data;
using Microsoft.AspNetCore.Mvc;

namespace Bookify.Web.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public CategoryController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public IActionResult Index()
        {
            //TODO: Add View Model For Category
            var Categories = _dbContext.Categories.Where(c=>c.IsDeleted == false).ToList(); // Get All Categories Where IsDeleted = False

            return View(Categories);
        }
    }
}

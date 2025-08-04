using Bookify.Web.Core.Models;
using Bookify.Web.Core.ViewModels;
using Bookify.Web.Data;
using Bookify.Web.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        {   // put AsNoTracking() to not track changes in this query, it is read-only operation.
            //TODO: Add View Model For Category
            var Categories = _dbContext.Categories/*.Where(c=>c.IsDeleted == false)*/.AsNoTracking().ToList(); // Get All Categories Where IsDeleted = False "when un Comm Where ==> is not show it in View Index"

            return View(Categories);
        }

        [HttpGet]
        [AjaxOnly]
        public IActionResult Create()
        {
            return PartialView("_Form");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CategoryFormViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var category = new Category
            {
                Name = model.Name,
            };
            _dbContext.Categories.Add(category);
            _dbContext.SaveChanges();
            return PartialView("_CategoryRow",category);
        }
        [HttpGet]
        [AjaxOnly] // this Attribute is used to ensure that this action can only be called via AJAX requests.
        public IActionResult Edit(int id)
        {
            var Categ = _dbContext.Categories.Find(id);

            if (Categ is null)
                return NotFound();

            var ViewModel = new CategoryFormViewModel
            {
                Id = Categ.Id,
                Name = Categ.Name,
            };
            return PartialView("_Form", ViewModel);


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CategoryFormViewModel model)
        {
            
            if (!ModelState.IsValid)
                return BadRequest();

            var Categ = _dbContext.Categories.Find(model.Id);

            if (Categ is null)
                return NotFound();

            Categ.Name = model.Name;
            Categ.LastUpdateOn = DateTime.Now;
            _dbContext.Categories.Update(Categ);
            _dbContext.SaveChanges();

            return PartialView("_CategoryRow", Categ);


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleStatus(int id)
        {

            var Categ = _dbContext.Categories.Find(id);
            if (Categ is null)
                return NotFound();
            ///if(Categ.IsDeleted)
            ///{
            ///    // If the category is already deleted, we can restore it
            ///    Categ.IsDeleted = false;
            ///}
            ///else
            ///{
            ///    // If the category is not deleted, we mark it as deleted
            ///    Categ.IsDeleted = true;
            ///}


            Categ.IsDeleted = !Categ.IsDeleted;// Toggle the IsDeleted status like if condition
            Categ.LastUpdateOn = DateTime.Now;

            _dbContext.Categories.Update(Categ);
            _dbContext.SaveChanges();
            return Ok(Categ.LastUpdateOn.ToString());

        }

        // Create Action To Prevent User To Add Duplicate Category Name by Client Side Validation
        public IActionResult AllowItem (CategoryFormViewModel model)
        {
            // Upgrade it to Select The Category Which Has The Same Name Comming From Model "To Upgrade ==> if You Edit Categoty but Not Change Name in Another Modules Which Comming in Next Days" 
            var category = _dbContext.Categories.SingleOrDefault(c=>c.Name==model.Name);
            var isAllowed =category is null || category.Id.Equals(model.Id); // If the category is null, it means the name does not exist in the database

            // If the category name already exists, return false ==> then it Run an Error Message
            return Json(isAllowed); // Return true if the category name does not exist, false otherwise            

        }
    }
}
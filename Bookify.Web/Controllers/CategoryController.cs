using Bookify.Web.Core.Models;
using Bookify.Web.Core.ViewModels;
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
            var Categories = _dbContext.Categories/*.Where(c=>c.IsDeleted == false)*/.ToList(); // Get All Categories Where IsDeleted = False "when un Comm Where ==> is not show it in View Index"

            return View(Categories);
        }

        public IActionResult Create()
        {
            return View("Form");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CategoryFormViewModel model)
        {
            if(!ModelState.IsValid)
                return View("Form",model);

            var category = new Category
            {
                Name= model.Name,
            };
            _dbContext.Categories.Add(category);
            _dbContext.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var Categ = _dbContext.Categories.Find(id);
            
            if(Categ is null)
                return NotFound();

            var ViewModel = new CategoryFormViewModel
            {
                Id = Categ.Id,
                Name = Categ.Name,
            };

            return View("Form", ViewModel);


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CategoryFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Form", model);

            var Categ = _dbContext.Categories.Find(model.Id);

            if (Categ is null)
                return NotFound();

            Categ.Name = model.Name;
            Categ.LastUpdateOn = DateTime.Now;
            _dbContext.Categories.Update(Categ);
            _dbContext.SaveChanges();

            return RedirectToAction(nameof(Index));


        }


    }
}

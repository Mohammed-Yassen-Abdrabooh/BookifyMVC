using Bookify.Web.Core.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bookify.Web.Controllers
{
    public class BookController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment; // Used to get the WWWroot path for file uploads "Saved Files Upload in wwwroot"
        private List<string> _allowedExtensions = new() { ".jpg", ".jpeg", ".png", ".gif" };
        private int _maxFileSize = 2097152; // 2 MB = 2 * 1024 * 1024;

        public BookController(ApplicationDbContext dbContext, IMapper mapper
              ,IWebHostEnvironment webHostEnvironment)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
           //var booksFormViewModel = PopulateViewModel();
            return View("Form", PopulateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(BooksFormViewModel model)
        {

            if (!ModelState.IsValid) 
            {
                model = PopulateViewModel(model);
                return View("Form",model);
            }
            var book = _mapper.Map<Book>(model);
            if(model.Image is not null)
            {
                var extension = Path.GetExtension(model.Image.FileName);
                if (!_allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError(nameof(model.Image), Errors.NotAllowedExtensionError);
                    return View("Form", PopulateViewModel());
                }
                if(model.Image.Length> _maxFileSize)
                {
                    ModelState.AddModelError(nameof(model.Image), Errors.MaxSizeError);
                    return View("Form", PopulateViewModel());
                }
                var imageName = $"{Guid.NewGuid()}{extension}";
                var path = Path.Combine($"{_webHostEnvironment.WebRootPath}/images/books", imageName);

                using var stream = System.IO.File.Create(path);
                model.Image.CopyTo(stream);
                book.ImageUrl = imageName;
            }
            foreach (var category in model.SelectedCategories)
            {
                var bookCategory = new BookCategory{ CategoryId = category };
                book.Categories.Add(bookCategory);
            }
            _dbContext.Books.Add(book);
            _dbContext.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var book = _dbContext.Books.Include(b => b.Categories).FirstOrDefault(b => b.Id == id);
            if (book is null)
                return NotFound();

            var model= _mapper.Map<BooksFormViewModel>(book);
            var booksFormViewModel = PopulateViewModel(model);
            booksFormViewModel.SelectedCategories = book.Categories.Select(c => c.CategoryId).ToList();
            return View("Form", PopulateViewModel(booksFormViewModel));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(BooksFormViewModel model)
        {

            if (!ModelState.IsValid)
            {
                model = PopulateViewModel(model);
                return View("Form", model);
            }
            var book = _dbContext.Books.Include(b => b.Categories).FirstOrDefault(b => b.Id == model.Id);
            if (book is null)
                return NotFound();

            if (model.Image is not null)
            {
                if (!string.IsNullOrEmpty(book.ImageUrl))
                {
                    var oldImagePath = Path.Combine($"{_webHostEnvironment.WebRootPath}/images/books", book.ImageUrl);
                    if (System.IO.File.Exists(oldImagePath))
                        System.IO.File.Delete(oldImagePath);
                }
                var extension = Path.GetExtension(model.Image.FileName);
                if (!_allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError(nameof(model.Image), Errors.NotAllowedExtensionError);
                    return View("Form", PopulateViewModel());
                }
                if (model.Image.Length > _maxFileSize)
                {
                    ModelState.AddModelError(nameof(model.Image), Errors.MaxSizeError);
                    return View("Form", PopulateViewModel());
                }
                var imageName = $"{Guid.NewGuid()}{extension}";
                var path = Path.Combine($"{_webHostEnvironment.WebRootPath}/images/books", imageName);

                using var stream = System.IO.File.Create(path);
                model.Image.CopyTo(stream);
                model.ImageUrl = imageName;
            }
            else if(model.Image is null && !string.IsNullOrEmpty(book.ImageUrl))
                model.ImageUrl = book.ImageUrl; // Keep the old image if no new image is uploaded

            book = _mapper.Map(model, book); // Update the book properties from the model
            book.LastUpdateOn = DateTime.Now; // Update the LastUpdateOn property
            foreach (var category in model.SelectedCategories)
            {
                var bookCategory = new BookCategory { CategoryId = category };
                book.Categories.Add(bookCategory);
            }

            _dbContext.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // Create Action To Prevent User To Add Duplicate Category Name by Client Side Validation
        public IActionResult AllowItem(BooksFormViewModel model)
        {
            // Upgrade it to Select The Category Which Has The Same Name Comming From Model "To Upgrade ==> if You Edit Categoty but Not Change Name in Another Modules Which Comming in Next Days" 
            var Book = _dbContext.Books.SingleOrDefault(b => b.Title == model.Title && b.AuthorId == model.AuthorId );
            var isAllowed = Book is null || Book.Id.Equals(model.Id); // If the category is null, it means the name does not exist in the database

            // If the category name already exists, return false ==> then it Run an Error Message
            return Json(isAllowed); // Return true if the category name does not exist, false otherwise            

        }

        private BooksFormViewModel PopulateViewModel(BooksFormViewModel? model = null)
        {
            var booksFormViewModel = (model is null)? new BooksFormViewModel() : model;
            var authors = _dbContext.Authors.Where(a => !a.IsDeleted).OrderBy(a => a.Name).ToList();
            var categories = _dbContext.Categories.Where(c => !c.IsDeleted).OrderBy(c => c.Name).ToList();
            booksFormViewModel.Authors = _mapper.Map<IEnumerable<SelectListItem>>(authors);
            booksFormViewModel.Categories = _mapper.Map<IEnumerable<SelectListItem>>(categories);

            return booksFormViewModel;


        }   
    }
}















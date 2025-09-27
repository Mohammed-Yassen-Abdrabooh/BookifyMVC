using Bookify.Web.Core.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.Linq.Dynamic.Core;

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

        public IActionResult Details(int id)
        {
            var book = _dbContext.Books
                                 .Include(a=>a.Author)
                                 .Include(bc => bc.Copies)
                                 .Include(c=>c.Categories)
                                 .ThenInclude(c=>c.Category)
                                 .SingleOrDefault(b=>b.Id == id);
            if(book is null)
                return NotFound();

            var viewModel = _mapper.Map<BookViewModel>(book);
            return View(viewModel);
        }

        public IActionResult Create()
        {
           //var booksFormViewModel = PopulateViewModel();
            return View("Form", PopulateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BooksFormViewModel model)
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
                var thumbPath = Path.Combine($"{_webHostEnvironment.WebRootPath}/images/books/thumb", imageName);

                using var stream = System.IO.File.Create(path);
                await model.Image.CopyToAsync(stream);
                stream.Dispose();

                book.ImageUrl = $"/images/books/{imageName}";
                book.ImageThumbnailUrl = $"/images/books/thumb/{imageName}";

                using var image = Image.Load(model.Image.OpenReadStream());
                var ratio = (float)image.Width / 200;
                var height = (int)(image.Height / ratio);
                image.Mutate(i => i.Resize(width: 200 , height: height));
                image.Save(thumbPath);
            }
            foreach (var category in model.SelectedCategories)
            {
                var bookCategory = new BookCategory{ CategoryId = category };
                book.Categories.Add(bookCategory);
            }
            _dbContext.Books.Add(book);
            _dbContext.SaveChanges();
            return RedirectToAction(nameof(Details), new { id = book.Id });
        }

        public IActionResult Edit(int id)
        {
            var book = _dbContext.Books.Include(b => b.Categories).FirstOrDefault(b => b.Id == id);
            if (book is null)
                return NotFound();

            var model= _mapper.Map<BooksFormViewModel>(book);
            var booksFormViewModel = PopulateViewModel(model);
            booksFormViewModel.SelectedCategories = book.Categories.Select(c => c.CategoryId).ToList();
            return View("Form", booksFormViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BooksFormViewModel model)
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
                    var oldImagePath =$"{_webHostEnvironment.WebRootPath}{book.ImageUrl}";
                    var oldThumbPath =$"{_webHostEnvironment.WebRootPath}{book.ImageThumbnailUrl}";
                    if (System.IO.File.Exists(oldImagePath))
                        System.IO.File.Delete(oldImagePath);

                    if (System.IO.File.Exists(oldThumbPath))
                        System.IO.File.Delete(oldThumbPath);
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
                var thumbPath = Path.Combine($"{_webHostEnvironment.WebRootPath}/images/books/thumb", imageName);

                using var stream = System.IO.File.Create(path);
                await model.Image.CopyToAsync(stream);
                stream.Dispose();

                model.ImageUrl = $"/images/books/{imageName}";
                model.ImageThumbnailUrl = $"/images/books/thumb/{imageName}";

                using var image = Image.Load(model.Image.OpenReadStream());
                var ratio = (float)image.Width / 200;
                var height = (int)(image.Height / ratio);
                image.Mutate(i => i.Resize(width: 200, height: height));
                image.Save(thumbPath);
            }
            else if(model.Image is null && !string.IsNullOrEmpty(book.ImageUrl))
            {
                model.ImageUrl = book.ImageUrl; // Keep the old image if no new image is uploaded
                model.ImageThumbnailUrl = book.ImageThumbnailUrl; // Keep the old Thumbnailimage if no new image is uploaded
            }

            book = _mapper.Map(model, book); // Update the book properties from the model
            book.LastUpdateOn = DateTime.Now; // Update the LastUpdateOn property
            foreach (var category in model.SelectedCategories)
            {
                var bookCategory = new BookCategory { CategoryId = category };
                book.Categories.Add(bookCategory);
            }

            _dbContext.SaveChanges();
            return RedirectToAction(nameof(Details), new { id = book.Id });
        }

        // Action To Get Books With Pagination For Usinge DataTable on server 
        [HttpPost]
        public IActionResult GetBooks()
        {
            var skip = int.Parse(Request.Form["start"]);
            var pageSize = int.Parse(Request.Form["length"]);

            var sortColumnIndex = int.Parse(Request.Form["order[0][column]"]);
            var sortColumnName = Request.Form[$"columns[{sortColumnIndex}][name]"];
            var sortColumnDirection = Request.Form["order[0][dir]"]; // asc or desc

            var searchValue = Request.Form["search[value]"]; // Search Value from (Search Box)
            IQueryable<Book> books = _dbContext.Books
                                               .Include(b=>b.Author)
                                               .Include(b=>b.Categories)
                                               .ThenInclude(c=>c.Category);

            if(!string.IsNullOrEmpty(searchValue))
                books= books.Where(b => b.Title.Contains(searchValue) || b.Author!.Name.Contains(searchValue) );
            // Using System.Linq.Dynamic.Core for dynamic sorting by using OrderBy() which get from this Lib
            // beacause it permit me to put there values as a String not as a Property of BookModel
            books = books.OrderBy($"{sortColumnName} {sortColumnDirection}"); 
            var data = books.Skip(skip).Take(pageSize).ToList();
            var mappedData = _mapper.Map<IEnumerable<BookViewModel>>(data);
            var recordsTotal = books.Count();

            var jsonData = new { recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = mappedData };
            return Ok(jsonData); 

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleStatus(int id)
        {

            var book = _dbContext.Books.Find(id);
            if (book is null)
                return NotFound();
            
            book.IsDeleted = !book.IsDeleted;// Toggle the IsDeleted status like if condition
            book.LastUpdateOn = DateTime.Now;

            _dbContext.Books.Update(book);
            _dbContext.SaveChanges();
            return Ok();

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















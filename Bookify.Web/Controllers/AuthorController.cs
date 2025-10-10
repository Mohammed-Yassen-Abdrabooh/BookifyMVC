
namespace Bookify.Web.Controllers
{
    [Authorize(Roles =AppRoles.Archive)]
    public class AuthorController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public AuthorController(ApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        public IActionResult Index()
        {
            var authors = _dbContext.Authors.AsNoTracking().ToList();

            var authorViewModels = _mapper.Map<IEnumerable<AuthorViewModel>>(authors); // Use AutoMapper to map the entities to view models
            return View(authorViewModels);
        }

        [HttpGet]
        [AjaxOnly]
        public IActionResult Create()
        {
            return PartialView("_Form");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(AuthorFormViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var author = _mapper.Map<Author>(model); // Use AutoMapper to map the model to the entity
            author.CreatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            _dbContext.Authors.Add(author);
            _dbContext.SaveChanges();

            var authorViewModel = _mapper.Map<AuthorViewModel>(author);

            return PartialView("_AuthorRow", authorViewModel);
        }


        [HttpGet]
        [AjaxOnly] // this Attribute is used to ensure that this action can only be called via AJAX requests.
        public IActionResult Edit(int id)
        {
            var Author = _dbContext.Authors.Find(id);

            if (Author is null)
                return NotFound();

            var ViewModel = _mapper.Map<AuthorFormViewModel>(Author);

            return PartialView("_Form", ViewModel);


        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(AuthorFormViewModel model)
        {

            if (!ModelState.IsValid)
                return BadRequest();

            var Author = _dbContext.Authors.Find(model.Id);

            if (Author is null)
                return NotFound();

            Author = _mapper.Map(model, Author);
            Author.LastUpdateById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            Author.LastUpdateOn = DateTime.Now;

            _dbContext.Authors.Update(Author);
            _dbContext.SaveChanges();

            var AuthorViewModel = _mapper.Map<AuthorViewModel>(Author);

            return PartialView("_AuthorRow", AuthorViewModel);


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleStatus(int id)
        {

            var Author = _dbContext.Authors.Find(id);
            if (Author is null)
                return NotFound();

            Author.IsDeleted = !Author.IsDeleted;// Toggle the IsDeleted status like if condition
            Author.LastUpdateOn = DateTime.Now;

            _dbContext.Authors.Update(Author);
            _dbContext.SaveChanges();
            return Ok(Author.LastUpdateOn.ToString());

        }



        // Create Action To Prevent User To Add Duplicate Author Name by Client Side Validation
        public IActionResult AllowItem(AuthorFormViewModel model)
        {
            // Upgrade it to Select The Author Which Has The Same Name Comming From Model "To Upgrade ==> if You Edit Categoty but Not Change Name in Another Modules Which Comming in Next Days" 
            var author = _dbContext.Authors.SingleOrDefault(a => a.Name == model.Name);
            var isAllowed = author is null || author.Id.Equals(model.Id); // If the Author is null, it means the name does not exist in the database

            // If the Author name already exists, return false ==> then it Run an Error Message
            return Json(isAllowed); // Return true if the Author name does not exist, false otherwise            

        }
    }
}

using Microsoft.AspNetCore.Mvc.Rendering;
using UoN.ExpressiveAnnotations.NetCore.Attributes;

namespace Bookify.Web.Core.ViewModels
{
    public class BooksFormViewModel
    {
        public int Id { get; set; }
        [MaxLength(500,ErrorMessage = Errors.MaxLengthError)]
        [Remote("AllowItem", "Book", AdditionalFields = "Id,AuthorId", ErrorMessage = Errors.DuplicatedBookError)]
        public string Title { get; set; } = null!;
        [Display(Name = "Author")]
        [Remote("AllowItem", "Book", AdditionalFields = "Id,Title", ErrorMessage = Errors.DuplicatedBookError)]
        public int AuthorId { get; set; } // Foreign Key to Author
        public IEnumerable<SelectListItem>? Authors { get; set; }
        [MaxLength(500, ErrorMessage = Errors.MaxLengthError)]
        public string? Publisher { get; set; } = null!;
        [Display(Name = "Publishing Date")]
        [AssertThat("PublishingDate <= Today()", ErrorMessage = Errors.NotAllowFutureDatesError)]
        public DateTime PublishingDate { get; set; } = DateTime.Now;
        public IFormFile? Image { get; set; }
        public string? ImageUrl { get; set; }
        [MaxLength(50, ErrorMessage = Errors.MaxLengthError)]
        public string Hall { get; set; } = null!;
        [Display(Name = "Is available for rental?")]
        public bool IsAvailableForRental { get; set; }
        public string Description { get; set; } = null!;

        // Navigation property for many-to-many relationship with Category
        [Display(Name = "Categories")]
        public IList<int> SelectedCategories { get; set; } = new List<int>();
        public IEnumerable<SelectListItem>? Categories { get; set; }
    }
}

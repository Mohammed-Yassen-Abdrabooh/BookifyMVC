namespace Bookify.Web.Core.Models
{
    [Index(nameof(Title),nameof(AuthorId), IsUnique = true)]
    public class Book : BaseModel
    {
        public int Id { get; set; }
        [MaxLength(500)]
        public string Title { get; set; } = null!;
        public int AuthorId { get; set; } // Foreign Key to Author
        public Author? Author { get; set; }
        [MaxLength(500)]
        public string? Publisher { get; set; } = null!;
        public DateTime PublishingDate { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImageThumbnailUrl { get; set; }
        public string? ImagePublicId { get; set; } // For Using in Cloudanary but I'm Not Use it
        [MaxLength(50)]
        public string Hall { get; set; } = null!;
        public bool IsAvailableForRental { get; set; }
        public string Description { get; set; } = null!;

        // Navigation property for many-to-many relationship with Category
        public ICollection<BookCategory> Categories { get; set; } = new List<BookCategory>();
        public ICollection<BookCopy> Copies { get; set; } = new List<BookCopy>();


    }
}

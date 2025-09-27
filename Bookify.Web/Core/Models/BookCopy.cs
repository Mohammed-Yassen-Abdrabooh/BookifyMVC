namespace Bookify.Web.Core.Models
{
    public class BookCopy : BaseModel
    {
        public int Id { get; set; }
        // Foreign Key to Book Nav Prop one To Many Relation "One Book Has Many Copies"
        public int BookId { get; set; }
        public Book? Book { get; set; }
        public bool IsAvailableForRental { get; set; }
        public int EditionNumber { get; set; }
        public int SerialNumber { get; set; }
    }
}

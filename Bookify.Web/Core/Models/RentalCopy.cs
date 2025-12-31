namespace Bookify.Web.Core.Models
{
    public class RentalCopy
    {
        // RentalCopy Id => is a Composite Key of RentalId and BookCopyId it Will be Configured in DbContext
        public int RentalId { get; set; }
        public Rental? Rental { get; set; }
        public int BookCopyId { get; set; }
        public BookCopy? BookCopy { get; set; }
        public DateTime RentalDate { get; set; } = DateTime.Today;
        //AddDays(7) => not correct to write numbers like 7 in The code directly Then You Should Use enum or Constant Class
        // So We Will Use RentalsConfiguration Enum and Must Cast it to int
        public DateTime EndDate { get; set; } = DateTime.Today.AddDays( (int) RentalsConfiguration.RentalDurationInDays);
        public DateTime? ReturnDate { get; set; }
        public DateTime? ExtendedOn { get; set; }
    }
}

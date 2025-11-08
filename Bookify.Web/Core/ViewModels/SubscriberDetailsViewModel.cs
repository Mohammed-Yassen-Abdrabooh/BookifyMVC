namespace Bookify.Web.Core.ViewModels
{
    public class SubscriberDetailsViewModel
    {
        public int Id { get; set; }
        public string? Key { get; set; }
        public string FullName { get; set; } = null!;
        public DateTime BirthOfDate { get; set; }
        public string NationalId { get; set; } = null!;
        public string MobileNumber { get; set; } = null!;
        public bool HasWhatsApp { get; set; }
        public string Email { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
        public string ImageThumbnailUrl { get; set; } = null!;
        public string Area { get; set; } = null!;
        public string Governorate { get; set; } = null!;
        public string Address { get; set; } = null!;
        public bool IsBlackListed { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}

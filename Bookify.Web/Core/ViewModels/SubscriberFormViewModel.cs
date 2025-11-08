using Microsoft.AspNetCore.Mvc.Rendering;
using UoN.ExpressiveAnnotations.NetCore.Attributes;

namespace Bookify.Web.Core.ViewModels
{
    public class SubscriberFormViewModel
    {
        //public int Id { get; set; }
        public string? Key { get; set; }
        [Display(Name = "First Name")]
        [MaxLength(100,ErrorMessage = Errors.MaxLengthError)]
        [RegularExpression(RegexPatterns.DenySpecialCharacters, ErrorMessage = Errors.DenySpecialCharacters)]
        public string FirstName { get; set; } = null!;
        [Display(Name = "Last Name")]
        [MaxLength(100, ErrorMessage = Errors.MaxLengthError)]
        [RegularExpression(RegexPatterns.DenySpecialCharacters, ErrorMessage = Errors.DenySpecialCharacters)]
        public string LastName { get; set; } = null!;
        [Display(Name = "Birth of Date")]
        [AssertThat("BirthOfDate <= Today()", ErrorMessage = Errors.NotAllowFutureDatesError)]
        public DateTime BirthOfDate { get; set; }=DateTime.Now;
        [Display(Name = "National Id")]
        [MaxLength(14, ErrorMessage = Errors.MaxLengthError)]
        [RegularExpression(RegexPatterns.NationalId,ErrorMessage = Errors.AllowEgyptianNationalIdError)]
        [Remote("AllowNationalId","Subscriber",AdditionalFields ="Key",ErrorMessage = Errors.DuplicatedError)]
        public string NationalId { get; set; } = null!;
        [Display(Name = "Mobile Number")]
        [MaxLength(11, ErrorMessage = Errors.MaxLengthError)]
        [RegularExpression(RegexPatterns.EgyptianMobileNumber, ErrorMessage = Errors.AllowEgyptianNumberError)]
        [Remote("AllowMobileNumber", "Subscriber", AdditionalFields = "Key", ErrorMessage = Errors.DuplicatedError)]

        public string MobileNumber { get; set; } = null!;
        [Display(Name = "Has WhatsApp?")]
        public bool HasWhatsApp { get; set; }
        [MaxLength(150, ErrorMessage = Errors.MaxLengthError)]
        [Remote("AllowEmail", "Subscriber", AdditionalFields = "Key", ErrorMessage = Errors.DuplicatedError)]
        [EmailAddress]
        public string Email { get; set; } = null!;
        [RequiredIf("Key == ''", ErrorMessage = Errors.EmptyImageError)]
        public IFormFile? Image { get; set; }

        //Nav property to Area
        public int AreaId { get; set; }
        public IEnumerable<SelectListItem>? Areas { get; set; } = new List<SelectListItem>();
        //Nav Property to Governorate
        public int GovernorateId { get; set; }
        public IEnumerable<SelectListItem>? Governorates { get; set; }

        [MaxLength(500, ErrorMessage = Errors.MaxLengthError)]
        public string Address { get; set; } = null!;
        [Display(Name = "Is BlackListed")]
        public bool IsBlackListed { get; set; }


        // these properties are take its Values from CreateAction in SubscriberController then it must be nullable here
        // because they are not takes its values by form inputs
        public string? ImageUrl { get; set; }
        public string? ImageThumbnailUrl { get; set; }

    }
}

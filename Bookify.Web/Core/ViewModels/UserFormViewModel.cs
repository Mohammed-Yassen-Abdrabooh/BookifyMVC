using MessagePack.Formatters;
using Microsoft.AspNetCore.Mvc.Rendering;
using UoN.ExpressiveAnnotations.NetCore.Attributes;

namespace Bookify.Web.Core.ViewModels
{
    public class UserFormViewModel
    {
        public string? Id { get; set; }

        [MaxLength(100, ErrorMessage = Errors.MaxLengthError)]
        [Display(Name = "Full Name")]
        [RegularExpression(RegexPatterns.CharactersOnly_Eng,ErrorMessage = Errors.OnlyEnglishLetters)]
        public string FullName { get; set; } = null!;

        [MaxLength(50,ErrorMessage =Errors.MaxLengthError)]
        [Remote("AllowUserName", "User", AdditionalFields = "Id", ErrorMessage = Errors.DuplicatedError)]
        [RegularExpression(RegexPatterns.UserName, ErrorMessage = Errors.InvalidUserName)]

        public string UserName { get; set; } = null!;

        [MaxLength(200, ErrorMessage = Errors.MaxLengthError)]
        [EmailAddress]
        [Remote("AllowEmail", "User", AdditionalFields = "Id", ErrorMessage = Errors.DuplicatedError)]
        public string Email { get; set; } = null!;

        [StringLength(100, ErrorMessage = Errors.MaxMinLengthError, MinimumLength = 8)]
        [DataType(DataType.Password)]
        [RegularExpression(RegexPatterns.Password,ErrorMessage = Errors.WeakPasswordError)]
        [RequiredIf("Id == null",ErrorMessage = Errors.RequiredFieldError)]
        public string? Password { get; set; } = null!;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = Errors.ConfirmPasswordNotMatchError)]
        [RequiredIf("Id == null", ErrorMessage = Errors.RequiredFieldError)]
        public string? ConfirmPassword { get; set; } = null!;

        // Navigation property for many-to-many relationship with Roles
        [Display(Name = "Roles")]
        public IList<string> SelectedRoles { get; set; } = new List<string>();
        public IEnumerable<SelectListItem>? Roles { get; set; }
    }
}


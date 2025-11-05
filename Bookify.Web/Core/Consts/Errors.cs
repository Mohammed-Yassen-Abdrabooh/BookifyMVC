namespace Bookify.Web.Core.Consts
{
    public static class Errors
    {
        public const string RequiredFieldError = "This Field is Required";
        public const string MaxLengthError = "The Length Cannot Be More Than {1} Char";
        public const string MaxMinLengthError = "The {0} must be at least {2} and at max {1} characters long.";
        public const string DuplicatedError = "Another Record {0} With The Same Name is Already Exist!!";
        public const string DuplicatedBookError = "Book With The Same Title is Already Exists With The Same Author !!";
        public const string NotAllowedExtensionError = "Only .png, .jpeg, .jpg, .gif files are Allowed";
        public const string MaxSizeError = "File Cannot be More Than 2 MB!";
        public const string NotAllowFutureDatesError = "Publishing Date Cannot be in The Future!!";
        public const string InvalidRangeError = "{0} Number Should be Between {1} To {2}";
        public const string ConfirmPasswordNotMatchError = "The password and confirmation password do not match.";
        public const string WeakPasswordError = "Passwords Contain an Uppercase Character, Lowercase Character, a Digit, and a Non-Alphanumeric Character. Passwords must be at Least 8 Characters Long.";
        public const string InvalidUserName = "UserName Can Only Contain Letters or Digits.";
        public const string OnlyEnglishLetters = "Only English letters are allowed.";
        public const string OnlyArabicLetters = "Only Arabic letters are allowed.";
        public const string OnlyNumbersAndLetters = "Only Arabic/English letters or digits are allowed.";
        public const string DenySpecialCharacters = "Special characters are not allowed.";
        public const string AllowEgyptianNumberError = "Egyptian Phone Numbers Only Allowed.";
        public const string AllowEgyptianNationalIdError = "Egyptian National Id Numbers Only Allowed.";
        public const string DuplicatedNationalIdError = "Another Subscriber Has This National Id.";
        public const string DuplicatedMobileNumberError = "Another Subscriber Has This Mobile Number.";
        public const string DuplicatedEmailError = "Another Subscriber Has This Email.";
        public const string EmptyImageError = "Must Use an Image For You.";



    }
}

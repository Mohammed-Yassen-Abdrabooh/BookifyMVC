namespace Bookify.Web.Core.ViewModels
{
    public class AuthorFormViewModel
    {

        public int Id { get; set; }
        [MaxLength(50, ErrorMessage = Errors.MaxLengthError), Display(Name = "Author")]
        // Remote Annotation it take ActionName,ControllerName, Error Message "دي بتخليك تتشك علي القيمه موجوده ولا لا ف الاكشن "
        [Remote("AllowItem", "Author", AdditionalFields = "Id", ErrorMessage = Errors.DuplicatedError)]
        public string Name { get; set; } = null!;
    }
}

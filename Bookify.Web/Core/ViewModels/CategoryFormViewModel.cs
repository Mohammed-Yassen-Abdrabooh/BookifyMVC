
namespace Bookify.Web.Core.ViewModels
{
    public class CategoryFormViewModel
    {
        public int Id { get; set; }
        [MaxLength(100, ErrorMessage = Errors.MaxLengthError), Display(Name = "Category")]
        // Remote Annotation it take ActionName,ControllerName, Error Message "دي بتخليك تتشك علي القيمه موجوده ولا لا ف الاكشن "
        [Remote("AllowItem", "Category", AdditionalFields = "Id", ErrorMessage = Errors.DuplicatedError)]
        public string Name { get; set; } = null!;
    }
}

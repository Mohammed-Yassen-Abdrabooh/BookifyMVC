using Microsoft.AspNetCore.Mvc;

namespace Bookify.Web.Core.ViewModels
{
    public class CategoryFormViewModel
    {
        public int Id { get; set; }
        [MaxLength(100,ErrorMessage = "Max Length Cannot Be More Than 100 Char") ]
        // Remote Annotation it take ActionName,ControllerName, Error Message "دي بتخليك تتشك علي القيمه موجوده ولا لا ف الاكشن "
        [Remote("AllowItem","Category",ErrorMessage ="The Category Name is Exist!!")]
        public string Name { get; set; }=null!;
    }
}

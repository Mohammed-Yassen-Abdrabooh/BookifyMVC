namespace Bookify.Web.Core.ViewModels
{
    public class CategoryFormViewModel
    {
        public int Id { get; set; }
        [MaxLength(100,ErrorMessage = "Max Length Cannot Be More Than 100 Char") ]
        public string Name { get; set; }=null!;
    }
}

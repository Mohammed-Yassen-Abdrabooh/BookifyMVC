// usinng namespace for data annotation but it imports here by GlobalUsings.cs
namespace Bookify.Web.Core.Models
{
    // Prevent DataBase To Store Dupplicate Values in Name Column "Now You can Not Add Dupplicate Category From DB or from Application"
    [Index(nameof(Name), IsUnique = true)]
    public class Category : BaseModel
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; } = null!; // Using null-forgiving operator to indicate that Name will not be null


    }
}

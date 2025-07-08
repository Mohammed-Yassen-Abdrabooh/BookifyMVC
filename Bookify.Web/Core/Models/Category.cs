// usinng namespace for data annotation but it imports here by GlobalUsings.cs
namespace Bookify.Web.Core.Models
{
    public class Category
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; } = null!; // Using null-forgiving operator to indicate that Name will not be null
        public bool IsDeleted { get; set; } // We Not Remove any Thing Frim Db but it get this flag = true and remove it in Showing only but it still in Db "We Select Categories Which Catagory.IsDeleted === False"
        public DateTime CreatedOn { get; set; } //= DateTime.Now// it not practice way , We Use onModelCreating in Dbcontext to Access This Entity and this prop To Add Default Value in SQL "GETDATE()"
        public DateTime? LastUpdateOn { get; set; }

    }
}

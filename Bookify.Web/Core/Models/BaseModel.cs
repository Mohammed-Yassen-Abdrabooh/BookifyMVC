namespace Bookify.Web.Core.Models
{
    public class BaseModel
    {
        public bool IsDeleted { get; set; } // We Not Remove any Thing Frim Db but it get this flag = true and remove it in Showing only but it still in Db "We Select Categories Which Catagory.IsDeleted === False"
        //Nav prop (one-to-many) to ApplicationUser To Get The Name of User Who Created or Updated this Entity
        public string? CreatedById { get; set; }
        public ApplicationUser? CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;//= DateTime.Now// it not practice way , We Use onModelCreating in Dbcontext to Access This Entity and this prop To Add Default Value in SQL "GETDATE()"
        public string? LastUpdateById { get; set; }
        public ApplicationUser? LastUpdateBy { get; set; }
        public DateTime? LastUpdateOn { get; set; } = DateTime.Now;
    }
}

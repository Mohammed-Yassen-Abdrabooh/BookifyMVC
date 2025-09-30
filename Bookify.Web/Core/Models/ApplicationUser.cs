using Microsoft.AspNetCore.Identity;

namespace Bookify.Web.Core.Models
{
    public class ApplicationUser: IdentityUser
    {
        [MaxLength(100)]
        public string FullName { get; set; } = null!;
        public bool IsDeleted { get; set; } 
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public DateTime? LastUpdateOn { get; set; }
    }
}

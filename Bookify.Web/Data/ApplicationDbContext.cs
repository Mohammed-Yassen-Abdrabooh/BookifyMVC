using Bookify.Web.Core.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }




        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Category>().Property(e=>e.CreatedOn).HasDefaultValueSql("GETDATE()"); // We Use it to Put Date now for this Prop in SQL
            base.OnModelCreating(builder);
        }
    }
}

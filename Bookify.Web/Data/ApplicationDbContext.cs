using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Bookify.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Category> Categories { get; set; }




        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Author>().Property(a => a.CreatedOn).HasDefaultValueSql("GETDATE()"); // We Use it to Put Date now for this Prop in SQL
            builder.Entity<Category>().Property(e => e.CreatedOn).HasDefaultValueSql("GETDATE()"); // We Use it to Put Date now for this Prop in SQL
            base.OnModelCreating(builder);
        }
    }
}

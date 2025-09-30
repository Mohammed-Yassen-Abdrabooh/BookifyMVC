using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Bookify.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<BookCategory> BookCategories { get; set; }
        public DbSet<BookCopy> BookCopies { get; set; }
        public DbSet<Category> Categories { get; set; }




        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Author>().Property(a => a.CreatedOn).HasDefaultValueSql("GETDATE()"); // We Use it to Put Date now for this Prop in SQL
            builder.Entity<Book>().Property(b => b.CreatedOn).HasDefaultValueSql("GETDATE()"); // We Use it to Put Date now for this Prop in SQL
            builder.Entity<BookCategory>().HasKey(e => new {e.BookId , e.CategoryId });// Composite Key for BookCategory
            builder.Entity<Category>().Property(e => e.CreatedOn).HasDefaultValueSql("GETDATE()"); // We Use it to Put Date now for this Prop in SQL   
            builder.HasSequence<int>("SerialNumber",schema:"Shared").StartsAt(1000001); // Create Sequence in SQL to use it in BookCopy SerialNumber
            builder.Entity<BookCopy>().Property(bc=>bc.SerialNumber).HasDefaultValueSql("NEXT VALUE FOR Shared.SerialNumber"); // Use Sequence in SerialNumber Prop
            base.OnModelCreating(builder);
        }
    }
}

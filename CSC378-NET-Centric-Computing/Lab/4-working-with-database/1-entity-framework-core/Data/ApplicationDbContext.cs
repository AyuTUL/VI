using _1_entity_framework_core.Models;
using Microsoft.EntityFrameworkCore;

namespace _1_entity_framework_core.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
    }
}
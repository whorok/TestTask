using Microsoft.EntityFrameworkCore;
using TestTask.Models.Entities;

namespace TestTask.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Organization> Organizations { get; set; }
    }
}

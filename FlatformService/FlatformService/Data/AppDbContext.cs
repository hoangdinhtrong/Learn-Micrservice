using FlatformService.Models;
using Microsoft.EntityFrameworkCore;

namespace FlatformService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt)
        {

        }

        public DbSet<Platform> Platforms { get; set; } = null!;
    }
}

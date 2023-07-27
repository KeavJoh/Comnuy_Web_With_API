using ComnuyWebWithAPI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ComnuyWebWithAPI.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Tool> Tools { get; set; }
        public DbSet<ToolGroup> ToolGroups { get; set; }
        public DbSet<ToolFavorites> ToolFavoritesFromUsers { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
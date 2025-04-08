// AppDbContext.cs
using Microsoft.EntityFrameworkCore;
using LightEditor2.Core.Models;


namespace LightEditor2.Core.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) 
        {    
        }

        public DbSet<Project> Projects { get; set; }
        public DbSet<SubGroup> SubGroups { get; set; }
        public DbSet<Slide> Slides { get; set; }
        public DbSet<Setting> Settings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Hier können Sie Beziehungen und weitere Konfigurationen definieren.
            modelBuilder.Entity<Project>()
                .HasMany(p => p.SubGroups)
                .WithOne(s => s.Project)
                .HasForeignKey(s => s.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SubGroup>()
                .HasMany(s => s.Slides)
                .WithOne()
                .HasForeignKey(sl => sl.SubGroupId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

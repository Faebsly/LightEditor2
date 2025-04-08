using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LightEditor2.Core.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            // Ermitteln Sie das aktuelle Arbeitsverzeichnis
            // und definieren Sie den Pfad zur SQLite-Datenbank
            var currentDir = Directory.GetCurrentDirectory();
            var dbPath = Path.Combine(currentDir, "appdb.db");

            optionsBuilder.UseSqlite($"Filename={dbPath}");
            return new AppDbContext(optionsBuilder.Options);
        }
    }
}

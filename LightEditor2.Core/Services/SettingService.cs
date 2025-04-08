// LightEditor2.Core/Services/SettingService.cs
using LightEditor2.Core.Data;
using LightEditor2.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LightEditor2.Core.Services
{
    public class SettingService : ISettingService
    {
        private readonly IDbContextFactory<AppDbContext> _dbContextFactory;
        private readonly ILogger<SettingService> _logger;

        public SettingService(IDbContextFactory<AppDbContext> dbContextFactory, ILogger<SettingService> logger)
        {
            _dbContextFactory = dbContextFactory;
            _logger = logger;
        }

        public async Task<string?> GetSettingAsync(string key)
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            try
            {
                var setting = await dbContext.Settings.FindAsync(key);
                return setting?.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Abrufen der Einstellung mit Key '{SettingKey}'.", key);
                return null; // <-- Explizites return im catch-Block
            }
            // Kein Code nach dem try-catch erlaubt, der kein return hat!
        }
        public async Task<bool> SetSettingAsync(string key, string value)
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            try
            {
                var existingSetting = await dbContext.Settings.FindAsync(key);
                if (existingSetting != null)
                {
                    existingSetting.Value = value;
                    dbContext.Settings.Update(existingSetting);
                }
                else
                {
                    var newSetting = new Setting { Key = key, Value = value };
                    await dbContext.Settings.AddAsync(newSetting);
                }
                await dbContext.SaveChangesAsync();
                return true; // <-- Return nach erfolgreicher Operation
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Setzen der Einstellung mit Key '{SettingKey}'.", key);
                return false; // <-- Explizites return im catch-Block
            }
            // Kein Code nach dem try-catch erlaubt, der kein return hat!
        }
    }
}
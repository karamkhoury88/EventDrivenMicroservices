using Microsoft.EntityFrameworkCore;

namespace InventoryService.Data
{
    public static class DataExtentions
    {
        public async static Task MigrateDbAsync(this WebApplication application)
        {
            using IServiceScope scope = application.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();
            await db.Database.MigrateAsync();
        }
    }
}

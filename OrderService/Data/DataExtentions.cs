using Microsoft.EntityFrameworkCore;

namespace OrderService.Data
{
    public static class DataExtentions
    {
        public async static Task MigrateDbAsync(this WebApplication application)
        {
            using IServiceScope scope = application.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
            await db.Database.MigrateAsync();
        }
    }
}

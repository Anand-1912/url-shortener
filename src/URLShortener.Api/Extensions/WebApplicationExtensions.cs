using Microsoft.EntityFrameworkCore;
using URLShortener.Api.Data;

namespace URLShortener.Api.Extensions
{
    public static class WebApplicationExtensions
    {
        public static void ApplyDatabaseMigrations(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            dbContext.Database.Migrate();
        }
    }
}

using Microsoft.EntityFrameworkCore;
using URLShortener.Api.Data;
using URLShortener.Api.Services;

namespace URLShortener.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            return services.AddDbContext<ApplicationDbContext>(
            dbContextOptions => dbContextOptions.UseSqlServer(configuration.GetConnectionString("Database"),
            sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorNumbersToAdd: null);
            }));
        }
        public static IServiceCollection AddUrlShortenerService(this IServiceCollection services)
        {
           return services.AddScoped<UrlShorteningService>();
        }
        public static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration)
        {
            return services.
                AddStackExchangeRedisCache
                (   
                    redisOptions => {
                    var connection = configuration.GetConnectionString("Redis");
                    redisOptions.Configuration = connection;
                });
        }
    }
}

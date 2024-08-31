using Carter;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using URLShortener.Api.Data;
using URLShortener.Api.Entities;
using URLShortener.Api.Models;
using URLShortener.Api.Services;

namespace URLShortener.Api.Endpoints
{
    public class UrlShortenerModule : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("api/shorten", async
            (
                ShortenUrlRequest request,
                UrlShorteningService urlShorteningService,
                ApplicationDbContext dbContext,
                IDistributedCache cache,
                HttpContext httpContext,
                CancellationToken cancellationToken
            ) =>
            {
                if (!Uri.TryCreate(request.Url, UriKind.Absolute, out _))
                {
                    return Results.BadRequest("The specified URL is invalid");
                }

                var code = await urlShorteningService.GenerateUniqueCode();

                var shortenedUrl = new ShortenedUrl
                {
                    Id = Guid.NewGuid(),
                    LongUrl = request.Url,
                    Code = code,
                    ShortUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}/api/{code}",
                    CreatedOnUtc = DateTime.UtcNow
                };

                dbContext.Add(shortenedUrl);

                await dbContext.SaveChangesAsync(cancellationToken);

                await cache.SetStringAsync(code, shortenedUrl.LongUrl, cancellationToken);

                return Results.Ok(shortenedUrl.ShortUrl);
            });

            app.MapGet("api/{code}", async
            (
                string code,
                ApplicationDbContext dbContext,
                IDistributedCache cache,
                CancellationToken cancellationToken
            ) =>
        {

            string longUrl = await cache.GetStringAsync(code, cancellationToken);

            if (string.IsNullOrEmpty(longUrl))
            {
                var shortenedUrl = await dbContext.ShortenedUrls.
                                    AsNoTracking().
                                    FirstOrDefaultAsync(s => s.Code == code, cancellationToken);

                if (shortenedUrl is null)
                {
                    return Results.NotFound();
                }

                longUrl = shortenedUrl.LongUrl;

                await cache.SetStringAsync(code, longUrl);
            }
                
            return Results.Redirect(longUrl);
        });
        }
    }
}

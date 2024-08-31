using Microsoft.EntityFrameworkCore;
using URLShortener.Api.Entities;
using URLShortener.Api.Services;

namespace URLShortener.Api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<ShortenedUrl> ShortenedUrls { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ShortenedUrl>(builder =>
            {
                builder.Property(s => s.Code).HasMaxLength(UrlShorteningService.NumberOfCharsInShortLink);
                builder.HasIndex(s => s.Code).IsUnique();
            });
        }
    }
}

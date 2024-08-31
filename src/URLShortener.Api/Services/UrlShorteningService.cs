using Microsoft.EntityFrameworkCore;
using URLShortener.Api.Data;

namespace URLShortener.Api.Services
{
    public class UrlShorteningService
    {
        public const int NumberOfCharsInShortLink = 7;
        
        private const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        
        private readonly Random _random = new Random();
        
        private readonly ApplicationDbContext _dbContext;

        public UrlShorteningService(ApplicationDbContext applicationDbContext)
        {
            _dbContext = applicationDbContext;
        }

        public async Task<string> GenerateUniqueCode()
        {
            var codeChars = new char[NumberOfCharsInShortLink];

            while (true)
            {
                for (var i = 0; i < NumberOfCharsInShortLink; i++)
                {
                    var randomIndex = _random.Next(Alphabet.Length - 1);
                    codeChars[i] = Alphabet[randomIndex];
                }
                var code = new string(codeChars);

                if (!await _dbContext.ShortenedUrls.AnyAsync(s => s.Code == code))
                {
                    return code;
                }
            }
        }
    }
}

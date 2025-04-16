
namespace Core.Services
{
    public class HashService : Services.IHashService
    {
        private readonly Services.IHashStrategy _hashStrategy;

        public HashService(Services.IHashStrategy hashStrategy)
        {
            _hashStrategy = hashStrategy;
        }

        public string GenerateHash(string? input)
        {
            return _hashStrategy.ComputeHash(input);
        }
    }
}
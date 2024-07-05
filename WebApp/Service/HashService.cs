using WebApp.Service.IService;

namespace WebApp.Service
{
    public class HashService : IHashService
    {
        private readonly IHashStrategy _hashStrategy;

        public HashService(IHashStrategy hashStrategy)
        {
            _hashStrategy = hashStrategy;
        }

        public string GenerateHash(string? input)
        {
            return _hashStrategy.ComputeHash(input);
        }
    }
}
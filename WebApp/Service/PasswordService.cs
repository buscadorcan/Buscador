using WebApp.Service.IService;

namespace WebApp.Service
{
    public class PasswordService : IPasswordService
    {
        private readonly IPasswordGenerationStrategy _passwordGenerationStrategy;

        public PasswordService(IPasswordGenerationStrategy passwordGenerationStrategy)
        {
            _passwordGenerationStrategy = passwordGenerationStrategy;
        }

        public string GenerateTemporaryPassword(int length)
        {
            return _passwordGenerationStrategy.GeneratePassword(length);
        }
    }
}
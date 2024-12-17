using WebApp.Service.IService;

namespace WebApp.Service
{
  public class PasswordService : IPasswordService
  {
    private readonly IPasswordGenerationStrategy _passwordGenerationStrategy;

    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="PasswordService"/>.
    /// </summary>
    /// <param name="passwordGenerationStrategy">La estrategia que se usará para generar contraseñas.</param>
    public PasswordService(IPasswordGenerationStrategy passwordGenerationStrategy)
    {
      _passwordGenerationStrategy = passwordGenerationStrategy;
    }

    /// <summary>
    /// Genera una contraseña temporal con la longitud especificada.
    /// </summary>
    /// <param name="length">La longitud de la contraseña temporal que se generará.</param>
    /// <returns>Una contraseña temporal generada según la estrategia configurada.</returns>
    public string GenerateTemporaryPassword(int length)
    {
      return _passwordGenerationStrategy.GeneratePassword(length);
    }
  }
}

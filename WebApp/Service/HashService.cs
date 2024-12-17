using WebApp.Service.IService;

namespace WebApp.Service
{
  public class HashService : IHashService
  {
    private readonly IHashStrategy _hashStrategy;

    /// <summary>
    /// Constructor de la clase HashService.
    /// </summary>
    /// <param name="hashStrategy">Estrategia de hash que se utilizará para calcular el hash.</param>
    public HashService(IHashStrategy hashStrategy)
    {
      _hashStrategy = hashStrategy;
    }

    /// <summary>
    /// Genera un hash para la entrada proporcionada.
    /// </summary>
    /// <param name="input">Entrada que se utilizará para generar el hash.</param>
    /// <returns>El valor del hash generado.</returns>
    public string GenerateHash(string? input)
    {
      return _hashStrategy.ComputeHash(input);
    }
  }
}

using SharedApp.Dtos;

namespace Core.Interfaces
{
    public interface IIpCoordinatesService
    {
        Task<CoordinatesDto> GetCoordinates(string ip);
    }
}

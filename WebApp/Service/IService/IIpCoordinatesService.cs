using SharedApp.Models.Dtos;

namespace WebApp.Service.IService
{
    public interface IIpCoordinatesService
    {
        Task<CoordinatesDto> GetCoordinates(string ip);
    }
}

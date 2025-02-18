using SharedApp.Models.Dtos;

namespace WebApp.Repositories.IRepositories
{
    public interface IEventTrackingRepository
    {
        bool Create(paAddEventTrackingDto data);
    }
}

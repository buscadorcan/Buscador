using Microsoft.AspNetCore.Components.Forms;

namespace Infractruture.Interfaces
{
    public interface IUtilitiesService
    {
        Task<string> UploadIconAsync(IBrowserFile file, int idONA);
    }
}

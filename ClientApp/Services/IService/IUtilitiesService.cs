using Microsoft.AspNetCore.Components.Forms;

namespace ClientApp.Services.IService
{
    public interface IUtilitiesService
    {
        Task<string> UploadIconAsync(IBrowserFile file);
    }
}

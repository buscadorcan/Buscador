using BlazorBootstrap;

namespace ClientApp.Services
{
    public class ToastService
    {
        public List<ToastMessage> Messages = new List<ToastMessage>();
        //public void CreateToastMessage(ToastType toastType, string message)
        //{
        //    var toastMessage = new ToastMessage
        //    {
        //        Type = toastType,
        //        Title = "Información del Servidor",
        //        HelpText = $"{DateTime.Now}",
        //        Message = message,
        //    };

        //    Messages.Add(toastMessage);
        //}
        public void CreateToastMessage(ToastType toastType, string message)
        {
            var toastMessage = new CustomToastMessage
            {
                Type = toastType,
                Title = "Información del Servidor",
                HelpText = $"{DateTime.Now}",
                Message = message,
                CssClass = "label valign-text-middle inter-regular-normal-comet-16px"
            };

            Messages.Add(toastMessage);
        }
        public void ClearMessages()
        {
            Messages.Clear();
        }
        public class CustomToastMessage : ToastMessage
        {
            public string CssClass { get; set; } = "label valign-text-middle inter-regular-normal-comet-16px";
        }

    }
}

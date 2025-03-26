using Microsoft.AspNetCore.SignalR;

namespace NhanVT_MVC.Pages.Hubs
{
    public class FunewsHub : Hub
    {
        public async Task NotifyChange()
        {
            await Clients.All.SendAsync("Change");
        }
    }
}

using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Web
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string msg)
        {
            await Clients.All.SendAsync("ReceiveMessage", msg);
        }
    }
}

using Microsoft.AspNetCore.SignalR;

namespace WebStore.Hubs;

public class ChatHub : Hub
{
    public async Task SendMessage(string message) => 
        await Clients.Others.SendAsync("MessageToClient", message);
}
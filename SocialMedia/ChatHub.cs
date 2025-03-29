using Microsoft.AspNetCore.SignalR;

public class ChatHub : Hub
{
    public async Task SendMessage(string senderId, string receiverId, string content)
    {
        await Clients.User(receiverId).SendAsync("ReceiveMessage", senderId, content);
    }
}

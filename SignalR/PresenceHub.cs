using Dating_App.Extentions;
using Microsoft.AspNetCore.SignalR;

namespace Dating_App.SignalR;

public class PresenceHub : Hub
{
    private readonly PresnceTracker _tracker;

    public PresenceHub(PresnceTracker tracker)
    {
        _tracker = tracker;
    }

    public override async Task OnConnectedAsync()
    {
        await _tracker.UserConnected(Context.User.GetUsername(), Context.ConnectionId);
        
        await Clients.Others.SendAsync("User online",Context.User.GetUsername());

        var currentUsers = await _tracker.GetOnlineUsers();
        await Clients.All.SendAsync("GetOnlineUsers", currentUsers);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
       await _tracker.UserDisConnected(Context.User.GetUsername(), Context.ConnectionId);
        await Clients.Others.SendAsync("UserIsOffline", Context.User.GetUsername());
        await base.OnDisconnectedAsync(exception);
        
        var currentUsers = await _tracker.GetOnlineUsers();
        await Clients.All.SendAsync("GetOnlineUsers", currentUsers);
    }
}
using Dating_App.Extentions;
using Microsoft.AspNetCore.SignalR;

namespace Dating_App.SignalR;

public class MessageHub : Hub
{
    public MessageHub()
    {
    }

    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var otherUser = httpContext.Request.Query["user"].ToString();
        var groupName = GetGroupName(Context.User.GetUsername(),otherUser);
        await Groups.AddToGroupAsync(Context.ConnectionId,groupName);
         
    }

    private string GetGroupName(string caller, string otherUser)
    {
        var stringCompare = string.CompareOrdinal(caller , otherUser) < 0;

        return stringCompare ? $"{caller}-{otherUser}" :$"{otherUser}-{caller}";
    }
}
namespace LlamaEngineHost.Hubs;

using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Ardalis.GuardClauses;

using LlamaEngineHost.Utilities;


public class ChatHub:Hub
{

    private readonly ILogger<ChatHub> _logger;
    private readonly IConfiguration _configuration;

    public ChatHub(
           ILogger<ChatHub> logger,
           IConfiguration configuration
       )
    {
        this._configuration = Guard.Against.Null(configuration);
        this._logger = Guard.Against.Null(logger);
    }

    //methods

 
    // When a user connects
    public override async Task OnConnectedAsync()
    {
       
        var user = Context.User?.Identity?.Name ?? "Unknown";
        OnlineUserTracker.OnlineUsers.TryAdd(Context.ConnectionId, user);
        await Clients.Caller.SendAsync("ReceiveSystemMessage",
            Context.ConnectionId,
            $"Welcome {user}");

        await base.OnConnectedAsync();
    }

    // User joins a group
    public async Task JoinGroup(string groupName)
    {
        var user = Context.User?.Identity?.Name ?? "Unknown";
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

        await Clients.Group(groupName).SendAsync("ReceiveSystemMessage",
            Context.ConnectionId,
            $"{user} joined group {groupName}");
    }

    // User leaves a group
    public async Task LeaveGroup(string groupName)
    {
        var user = Context.User?.Identity?.Name ?? "Unknown";
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

        await Clients.Group(groupName).SendAsync("ReceiveSystemMessage",
            Context.ConnectionId,
            $"{user} left group {groupName}");
    }

    // Send message to a specific group
    public async Task SendMessageToGroup(string groupName, string user, string message)
    {
        await Clients.Group(groupName).SendAsync("ReceiveMessage", user, message);
    }

    // When a user disconnects
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
         var user = Context.User?.Identity?.Name ?? "Unknown";
         OnlineUserTracker.OnlineUsers.TryRemove(Context.ConnectionId, out _);
        await Clients.All.SendAsync("ReceiveSystemMessage",
            Context.ConnectionId,
            $"{user } leave group");

        await base.OnDisconnectedAsync(exception);
    }

}// end class



    


    


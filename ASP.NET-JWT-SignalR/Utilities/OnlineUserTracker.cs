namespace LlamaEngineHost.Utilities;

using System.Collections.Concurrent;

public static class OnlineUserTracker
{
    // Key = ConnectionId, Value = UserName (or email, or userId)
    public static ConcurrentDictionary<string, string> OnlineUsers 
        = new ConcurrentDictionary<string, string>();
}

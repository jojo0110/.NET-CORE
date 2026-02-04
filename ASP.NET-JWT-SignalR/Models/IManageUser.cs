namespace LlamaEngineHost.Models;
using System.Security.Claims;

public interface IManageUser
{
    Task<List<Claim>> CheckUser(LoginRequestDTO loginRequest, string sessionId);
}
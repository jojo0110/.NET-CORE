namespace LlamaEngineHost.Utilities;

using LlamaEngineHost.Models;
public interface IAuthService
{
    Task<string> AuthenticateAsync(LoginRequestDTO loginRequest);
    Task<bool> UpdateUserData(LoginRequestDTO loginRequest);
}
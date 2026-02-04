namespace LlamaEngineHost.Models;
using Ardalis.GuardClauses;
using System.Security.Claims;

public class ManageUser:IManageUser
{
    private readonly ILogger<ManageUser> _logger;
    private readonly IConfiguration _configuration;

    public ManageUser(ILogger<ManageUser> logger, IConfiguration configuration)
    {
        this._configuration = Guard.Against.Null(configuration);
        this._logger = Guard.Against.Null(logger);
    }

    public async Task<List<Claim>> CheckUser(LoginRequestDTO loginRequest, string sessionId)
    {
        try
        {
            var claims = new List<Claim>();
         
            claims.Add(new Claim(ClaimTypes.Role, "User"));
            claims.Add(new Claim(ClaimTypes.Email, loginRequest.Email));
            claims.Add(new Claim(ClaimTypes.Name, loginRequest.Name));
            claims.Add(new Claim(ClaimTypes.NameIdentifier, sessionId));
        

            await Task.Run(() => { });

            return claims;
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex.Message, ex);
            throw;
        }
    }


}
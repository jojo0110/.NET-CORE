namespace LlamaEngineHost.Utilities;
using System;
using System.Security.Claims;
using System.Text;
using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

using LlamaEngineHost.Models;

public class AuthService:IAuthService
{
     private readonly ILogger<AuthService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IManageUser _manageUser;

    public AuthService(
    ILogger<AuthService> logger,
    IConfiguration configuration,
    IManageUser manageUser
    )
    {
        this._configuration = Guard.Against.Null(configuration);
        this._logger = Guard.Against.Null(logger);
        this._manageUser = manageUser;
    }

     public async Task<string> AuthenticateAsync(LoginRequestDTO loginRequest)
        {
            try
            {
                string sessionId = Guid.NewGuid().ToString();

                Guard.Against.NullOrEmpty(loginRequest.Email);
                Guard.Against.NullOrEmpty(loginRequest.Name);

                JwtOptions jwtOptions = Guard.Against.Null(
                    this._configuration.GetSection(nameof(JwtOptions)).Get<JwtOptions>()
                );

                SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(jwtOptions.Key));
                SigningCredentials credentials = new(securityKey, SecurityAlgorithms.HmacSha256);
                DateTime expiration = DateTime.Now.AddDays(Convert.ToDouble(jwtOptions.ExpireDays));

                var claims =
                    await this._manageUser.CheckUser(loginRequest, sessionId)
                    ?? throw new ArgumentException("Invalid user");

                SecurityTokenDescriptor tokenDescriptor = new()
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = expiration,
                    SigningCredentials = credentials,
                    Issuer = jwtOptions.Issuer,
                    Audience = jwtOptions.Audience,
                };

                JsonWebTokenHandler tokenHandler = new();
                string tokenString = tokenHandler.CreateToken(tokenDescriptor);

                return tokenString;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex.Message, ex);
                throw;
            }
        }


        
    public async Task<bool> UpdateUserData(LoginRequestDTO loginRequest)
    {
        
        return false;

    }


}
using System.Net.Http;
using System.Reflection;
using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;

using System.Net.Http.Json;

using Ardalis.GuardClauses;
using LlamaEngineHost.Models;
using LlamaEngineHost.Utilities;

namespace LlamaEngineHost.Controllers
{
    public class HomeController
 : Controller
    {
       
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;

        private readonly IAuthService _authService;


        public HomeController
(
            ILogger<HomeController> logger,
            IConfiguration configuration,
            IAuthService authService
        )
        {
           
            _logger = logger;
            _configuration = configuration;
            _authService=authService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var count = OnlineUserTracker.OnlineUsers.Count;
            ViewBag.users = count;
            ViewBag.Groups = GroupDTO.All;
            return View(new LoginRequestDTO());
          
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginRequestDTO model)
        {
          
            if (!ModelState.IsValid)
            {
                
                return View(model);
            }
            try
            {
                string cleanEmail=string.Concat(model.Email.Where(c => !char.IsWhiteSpace(c)));
                string cleanName=string.Concat(model.Name.Where(c => !char.IsWhiteSpace(c)));
                int group=model.GroupNumber;
                var response=await _authService.AuthenticateAsync(new LoginRequestDTO
                    {
                        Email = cleanEmail,
                        Name = cleanName,
                        GroupNumber = group,
                    });
                Guard.Against.NullOrEmpty(response);

               
                var token = response;
                Response.Cookies.Append(
                    _configuration["jwtToken"],
                    token,
                    new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.None,
                        Path = "/"

                    }
                );
                
                return RedirectToAction("Index", "Chat", new {groupNumber=group});
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex.Message, ex);
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            Response.Cookies.Delete(_configuration["jwtToken"], new CookieOptions
            {
                Path = "/",
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None
            });

            return RedirectToAction("Index", "Home");
        }

    }
}

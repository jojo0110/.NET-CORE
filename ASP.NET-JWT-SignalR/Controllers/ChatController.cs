namespace LlamaEngineHost.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

using LlamaEngineHost.Models;


[Authorize(Roles = "User")]
public class ChatController:Controller
{
    [HttpGet]
        public IActionResult Index(int groupNumber)
        {
            var group = GroupDTO.All.FirstOrDefault(g => g.GroupNumber == groupNumber);

            ViewBag.userGroup = group.GroupName;

            var model = new ChatViewModel { 
                    loginRequestDTO=new LoginRequestDTO
                        {
                            Email = User.FindFirst(ClaimTypes.Email)?.Value,
                            Name = User.FindFirst(ClaimTypes.Name)?.Value,
                            GroupNumber=groupNumber
                        },
                    leftPanelViewModel = GetLeftPanelModel(groupNumber) 
                };
            return View(model);
        }
    
    [HttpGet]
    public IActionResult LoadLeftPanel(int groupNumber)
    {
        var model = GetLeftPanelModel(groupNumber);
        return PartialView("_LeftPanel", model);
    }


private LeftPanelViewModel GetLeftPanelModel(int  groupNumber)
 { 
     var group = GroupDTO.All.FirstOrDefault(g => g.GroupNumber == groupNumber);
    
    return new LeftPanelViewModel 
    { Group = group.GroupName, 
    Description = ""};
 }
}
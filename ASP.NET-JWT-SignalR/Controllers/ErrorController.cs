namespace LlamaEngineHost.Controllers;
using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Mvc;
public class ErrorController: Controller
{
    public IActionResult Index() { return View(); }

}
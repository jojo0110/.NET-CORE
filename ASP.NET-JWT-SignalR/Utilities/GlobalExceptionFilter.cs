namespace LlamaEngineHost.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;


public class GlobalExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        context.Result = new RedirectToActionResult("Index", "Error", null);
        context.ExceptionHandled = true;
    }
}


using LlamaEngineHost.Extension;
using LlamaEngineHost.Utilities;
using LlamaEngineHost.Hubs;
using Microsoft.Extensions.FileProviders;
using Serilog;
using Azure.Identity;
using Azure.Extensions.AspNetCore.Configuration.Secrets;


var builder = WebApplication.CreateBuilder(args);

//Developers: Make sure you have user secrets JwtOptions:Key setup in your local 
var keyVaultUrl = builder.Configuration["KeyVaultUrl"];
var key = builder.Configuration["jwtOptions:Key"]
          ?? throw new Exception("JWT signing key not found");


if (!string.IsNullOrWhiteSpace(keyVaultUrl))
{
    builder.Configuration.AddAzureKeyVault(
        new Uri(keyVaultUrl),
        new DefaultAzureCredential());
}


builder.Logging.ClearProviders();
builder.Host.UseSerilog(
    (context, services, loggerConfiguration) =>
        loggerConfiguration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
);


builder.Services.AddHosting(builder.Configuration);
builder.Services.AddScope(builder.Configuration);
builder.Services.AddJWTAuth(builder.Configuration,key);
builder.Services.AddSignalR(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseStaticFiles();


/* No Swagger Here. It is MVC
app.UseSwagger();
app.UseSwaggerUI(x =>
{
    x.SwaggerEndpoint("/swagger/v2/swagger.json", $":LlamaEngineHost API V2");
    x.RoutePrefix = "swagger";
});
*/

app.UseRouting();
app.UseAuthentication();
app.UseMiddleware<JwtLoggingMiddleware>();
app.UseAuthorization();
app.MapControllerRoute(
                   name: "Default",
                   pattern: "{controller=Home}/{action=Index}/{id?}"
               );

app.UseSerilogRequestLogging(); 

app.MapHub<ChatHub>("/chatHub");



 Console.WriteLine("Starting HTTPS server on port 7063...");
 Console.WriteLine("Starting HTTP server on port 5231...");
app.Run();



using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddApplicationPart(typeof(CyberClub.Controllers.Api.GeneralController).Assembly); 
builder.Services.AddEndpointsApiExplorer(); 

var app = builder.Build();

app.UseRouting();

app.UseAuthorization();

app.MapControllers(); 

app.Run();

namespace CyberClub.Api
{
    public partial class Program { }
}

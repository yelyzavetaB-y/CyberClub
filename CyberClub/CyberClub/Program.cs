using CyberClub.Domain.Interfaces;
using CyberClub.Domain.Models;
using CyberClub.Domain.Services;
using CyberClub.Domain.Validators;
using CyberClub.Infrastructure.DBService;
using CyberClub.Infrastructure.Interfaces;
using CyberClub.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<QueryBuilder>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<UserService>();
builder.Services.AddSingleton<IConnectionStringProvider, ConnectionStringProvider>();
builder.Services.AddScoped<IZoneRepository, ZoneRepository>();
builder.Services.AddScoped<ZoneService>();
builder.Services.AddScoped<ISeatRepository, SeatRepository>();
builder.Services.AddScoped<SeatService>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<BookingService>();
builder.Services.AddScoped<TournamentService>();
builder.Services.AddScoped<ITournamentRepository, TournamentRepository>();
builder.Services.AddScoped<IBookingApiRepository, BookingApiRepository>();
builder.Services.AddScoped<IBookingService, BookingServiceApi>();
builder.Services.AddScoped<ICustomValidator<Booking>, BookSeatValidator>();
builder.Services.AddControllersWithViews();
builder.Services.AddSession();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
  pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var userService = scope.ServiceProvider.GetRequiredService<UserService>();

    var existing = await userService.FindByEmailAsync("manager@cyberclub.com");

    if (existing == null)
    {
        var managerUser = new User
        {
            Email = "manager@cyberclub.com",
            FullName = "Manager",
            HashPassword = "Manager123",
            RoleId = 2,
            Profile = new UserProfile
            {
                PhoneNumber = "+380987541657",
                DOB = new DateTime(1992, 08, 12)
            }
        };

        await userService.AddUserAsync(managerUser); 
    }
}



app.Run();


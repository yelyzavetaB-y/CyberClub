using Moq;
using Xunit;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CyberClub.Controllers.Auth;
using CyberClub.Domain.Models;
using CyberClub.Domain.Interfaces;
using CyberClub.ViewModels;
using CyberClub.Domain.Services;

namespace UnitTests.Controllers.AuthController
{
    public class AuthControllerTests
    {
        [Fact]
        public async Task Login_ReturnsView_WithModelError_WhenUserNotFound()
        {
           
            var userRepo = new Mock<IUserRepository>();
            var mockUserService = new Mock<UserService>(userRepo.Object); 

            var controller = new CyberClub.Controllers.Auth.AuthController(mockUserService.Object);

            var model = new LoginViewModel
            {
                Email = "notfound@example.com",
                Password = "wrong"
            };

            userRepo.Setup(r => r.FindByEmailAsync(model.Email))
                    .ReturnsAsync((User)null);

            var result = await controller.Login(model);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(model, viewResult.Model);
            Assert.False(controller.ModelState.IsValid);
            Assert.True(controller.ModelState.ContainsKey(string.Empty));
        }
    }
}

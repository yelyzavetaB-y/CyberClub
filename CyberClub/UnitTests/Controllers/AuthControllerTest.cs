using CyberClub.Controllers.Auth;
using CyberClub.Domain.Interfaces;
using CyberClub.Domain.Models;
using CyberClub.Domain.Services;
using CyberClub.Helper;
using CyberClub.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitTests.Helper;

namespace UnitTests.Controllers
{
    public class AuthControllerTest
    {
        private readonly Mock<IUserRepository> _mockUserRepo;
        private readonly AuthController _controller;

        public AuthControllerTest()
        {
            _mockUserRepo = new Mock<IUserRepository>();
            var userService = new UserService(_mockUserRepo.Object);

            _controller = new AuthController(userService);

            var context = new DefaultHttpContext();
            context.Session = new DummySession();
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = context
            };
        }

        [Fact]
        public void Login_Get_WithUserInSession_RedirectsToPanel()
        {
            _controller.HttpContext.Session.SetString("User", "user@example.com");

            var result = _controller.Login();

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Panel", redirect.ActionName);
            Assert.Equal("Customer", redirect.ControllerName);
        }

        [Fact]
        public void Login_Get_WithoutSession_ReturnsView()
        {
            var result = _controller.Login();

            var view = Assert.IsType<ViewResult>(result);
            Assert.Null(view.ViewName);
        }

        [Fact]
        public async Task Login_Post_ValidCredentials_RedirectsToCustomerPanel()
        {
            var loginModel = new LoginViewModel
            {
                Email = "test@example.com",
                Password = "Password123"
            };

            string salt = "salt";
            string hashed = SecurityHelper.HashPassword(loginModel.Password, salt, 10101, 70);

            _mockUserRepo.Setup(r => r.FindByEmailAsync(loginModel.Email))
                .ReturnsAsync(new User
                {
                    Id = 1,
                    Email = loginModel.Email,
                    HashPassword = hashed,
                    Salt = salt
                });

            var result = await _controller.Login(loginModel);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Panel", redirect.ActionName);
            Assert.Equal("Customer", redirect.ControllerName);
        }

        [Fact]
        public async Task Login_Post_InvalidCredentials_ReturnsViewWithError()
        {
            var model = new LoginViewModel { Email = "wrong@example.com", Password = "wrongpass" };

            _mockUserRepo.Setup(r => r.FindByEmailAsync(model.Email))
                .ReturnsAsync((User)null);

            var result = await _controller.Login(model);

            var view = Assert.IsType<ViewResult>(result);
            Assert.Equal(model, view.Model);
            Assert.False(_controller.ModelState.IsValid);
        }

        [Fact]
        public void Signup_Get_WithSession_RedirectsToPanel()
        {
            _controller.HttpContext.Session.SetString("User", "user@example.com");

            var result = _controller.Signup();

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Panel", redirect.ActionName);
            Assert.Equal("Customer", redirect.ControllerName);
        }

        [Fact]
        public void Signup_Get_WithoutSession_ReturnsView()
        {
            var result = _controller.Signup();

            var view = Assert.IsType<ViewResult>(result);
            Assert.Null(view.ViewName);
        }

        [Fact]
        public async Task Logout_ClearsSessionAndRedirectsToLogin()
        {
            _controller.HttpContext.Session.SetString("User", "someuser");

            var result = await _controller.Logout();

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Login", redirect.ActionName);
            Assert.Equal("Auth", redirect.ControllerName);
        }


    }
}

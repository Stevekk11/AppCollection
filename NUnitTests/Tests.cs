using NUnit.Framework;
using AppCollection.Controllers;
using AppCollection.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using AppCollection.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;

namespace NUnitTests;

[TestFixture]
public class AccountControllerTests
{
    private ApplicationDbContext _context;
    private AccountController _controller;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;
        _context = new ApplicationDbContext(options);
        _controller = new AccountController(_context);

        // Setup controller context
        var httpContext = new DefaultHttpContext();
        _controller.ControllerContext = new ControllerContext()
        {
            HttpContext = httpContext
        };
        var urlHelperMock = new Mock<IUrlHelper>();
        urlHelperMock
            .Setup(x => x.Action(It.IsAny<UrlActionContext>()))
            .Returns("/");
        _controller.Url = urlHelperMock.Object;

    }

    [Test]
    public void Register_WithValidModel_RedirectsToLogin()
    {
        // Arrange
        var model = new Login { Username = "testuser", Password = "password123", Usertype = 2 };

        // Act
        var result = _controller.Register(model) as RedirectToActionResult;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.ActionName, Is.EqualTo("Login"));
    }

    [Test]
    public void Register_WithExistingUsername_ReturnsViewWithError()
    {
        // Arrange
        var existingUser = new Login { Username = "existing", Password = "password123", Usertype = 2 };
        _context.Logins.Add(existingUser);
        _context.SaveChanges();

        var model = new Login { Username = "existing", Password = "newpassword", Usertype = 2 };

        // Act
        var result = _controller.Register(model) as ViewResult;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(_controller.ModelState.IsValid, Is.False);
        Assert.That(_controller.ModelState["Username"].Errors[0].ErrorMessage, Is.EqualTo("Username already exists."));
    }

    [Test]
    public async Task Login_WithValidCredentials_RedirectsToHome()
    {
        // Arrange
        var user = new Login { Username = "testuser", Password = PasswordHelper.HashPassword("password123"), Usertype = 2 };
        _context.Logins.Add(user);
        _context.SaveChanges();

        var model = new Login { Username = "testuser", Password = "password123", Usertype = 2 };

        // Setup authentication mock
        var authServiceMock = new Mock<IAuthenticationService>();
        authServiceMock
            .Setup(x => x.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()))
            .Returns(Task.CompletedTask);

        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock
            .Setup(x => x.GetService(typeof(IAuthenticationService)))
            .Returns(authServiceMock.Object);

        _controller.ControllerContext.HttpContext.RequestServices = serviceProviderMock.Object;

        // Act
        var result = await _controller.Login(model) as RedirectToActionResult;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.ActionName, Is.EqualTo("Index"));
        Assert.That(result.ControllerName, Is.EqualTo("Home"));
    }
    
    

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
        _controller.Dispose();
    }
}
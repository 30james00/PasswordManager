using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using PasswordManager.Application.Security.Token;

namespace Application.UnitTests.Security.Token;

public class UserAccessorTests
{
    [Test]
    public void GetUserId_User_UserId()
    {
        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier, "id"),
        };
        var identity = new ClaimsIdentity(claims, "Test");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        var mockPrincipal = new Mock<IPrincipal>();
        mockPrincipal.Setup(x => x.Identity).Returns(identity);
        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.Setup(m => m.User).Returns(claimsPrincipal);
        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        mockHttpContextAccessor.Setup(accessor => accessor.HttpContext).Returns(mockHttpContext.Object);
        var userAccessor = new UserAccessor(mockHttpContextAccessor.Object);

        var userId = userAccessor.GetUserId();

        userId.Should().Be("id");
    }    
    
    [Test]
    public void GetUserId_NoLoggedUser_Null()
    {
        var claims = new List<Claim>();
        var identity = new ClaimsIdentity(claims, "Test");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        var mockPrincipal = new Mock<IPrincipal>();
        mockPrincipal.Setup(x => x.Identity).Returns(identity);
        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.Setup(m => m.User).Returns(claimsPrincipal);
        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        mockHttpContextAccessor.Setup(accessor => accessor.HttpContext).Returns(mockHttpContext.Object);
        var userAccessor = new UserAccessor(mockHttpContextAccessor.Object);

        var userId = userAccessor.GetUserId();

        userId.Should().BeNull();
    }
}
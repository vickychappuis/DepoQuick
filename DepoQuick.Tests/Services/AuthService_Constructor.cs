using DepoQuick.Models;
using DepoQuick.DataAccess.Repos;
using DepoQuick.Backend.Services;
using DepoQuick.DataAccess;
using Microsoft.Extensions.DependencyInjection;

namespace DepoQuick.Tests.Services;

[TestClass]
public class AuthService_Constructor
{
    private IRepo<User, string> _userRepo;
    
    [TestInitialize]
    public void TestInit()
    {
        var testsContext = new TestProgram();
        using var scope = testsContext.ServiceProvider.CreateScope();
        _userRepo = scope.ServiceProvider.GetRequiredService<IRepo<User, string>>();
    }

    [TestMethod]
    public void Constructor_DatabaseWithUser_ShouldHaveNoAuthedUser()
    {
        _userRepo.Add(new User( "John Doe", "john@doe.com", "TestPass1#", true));
        
        AuthService authService = new AuthService(_userRepo);
        
        Assert.IsNull(authService.AuthedUser);
    }
    
    [TestMethod]
    public void Constructor_NoAdminExists_ShouldSetAdminExistsToFalse()
    {
        AuthService authService = new AuthService(_userRepo);
        
        Assert.IsFalse(authService.AdminExists);
    }
    
    [TestMethod]
    public void Constructor_AdminExists_ShouldSetAdminExistsToTrue()
    {
        _userRepo.Add(new User("John Doe", "john@doe.com", "TestPass1#", true));
        
        AuthService authService = new AuthService(_userRepo);
        
        Assert.IsTrue(authService.AdminExists);
    }
}
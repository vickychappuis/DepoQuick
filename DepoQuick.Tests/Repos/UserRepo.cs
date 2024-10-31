using DepoQuick.DataAccess;
using DepoQuick.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DepoQuick.Tests.Repos;

[TestClass]
public class UserRepo
{
    private IDbContextFactory<Context> _contextFactory;
    private DataAccess.Repos.UserRepo _userRepo;

    [TestInitialize]
    public void TestInit()
    {
        var testsContext = new TestProgram();
        using var scope = testsContext.ServiceProvider.CreateScope();
        _contextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<Context>>();
        _userRepo = new DataAccess.Repos.UserRepo(_contextFactory);
    }

    [TestMethod]
    public void Add_ShouldAddUser()
    {
        User user = _userRepo.Add(new User("john", "john@doe.com", "testPass!1", true));

        Assert.AreEqual(1, user.UserId);
        Assert.AreEqual(user, _userRepo.GetAll()[0]);
    }

    [TestMethod]
    public void Delete_ShouldDeleteUser()
    {
        User user = _userRepo.Add(new User("john", "john@doe.com", "testPass!1", true));

        _userRepo.Delete(user.UserId);

        Assert.AreEqual(0, _userRepo.GetAll().Count);
    }

    [TestMethod]
    public void Delete_NotFoundUser_ShouldNotDeleteUser()
    {
        User user1 = _userRepo.Add(new User("john", "john@doe.com", "testPass!1", true));

        _userRepo.Delete(user1.UserId + 1);

        Assert.AreEqual(1, _userRepo.GetAll().Count);
    }

    [TestMethod]
    public void GetAll_ShouldReturnAllUsers()
    {
        User user1 = _userRepo.Add(new User("john", "john@doe.com", "testPass!1", true));
        User user2 = _userRepo.Add(new User("jane", "jane@doe.com", "testPass!1", false));
        
        var users = _userRepo.GetAll();

        Assert.AreEqual(2, users.Count);
        Assert.AreEqual(user1, users[0]);
        Assert.AreEqual(user2, users[1]);
       }

    [TestMethod]
    public void Get_FoundUser_ShouldReturnUser()
    {
        User user = _userRepo.Add(new User("john", "john@doe.com", "testPass!1", true));

        var foundUser = _userRepo.Get(user.UserId);

        Assert.AreEqual(user, foundUser);
    }

    [TestMethod]
    public void Get_NotFoundUser_ShouldReturnNull()
    {
        User user = _userRepo.Add(new User("john", "john@doe.com", "testPass!1", true));

        var foundUser = _userRepo.Get(user.UserId + 1);

        Assert.IsNull(foundUser);
    }

    [TestMethod]
    public void Update_ShouldUpdateUser()
    {
        User user = _userRepo.Add(new User("john", "john@doe.com", "testPass!1", true));

        user.Name = "jane";
        user.Email = "jane@doe.com";

        _userRepo.Update(user);

        var updatedUser = _userRepo.Get(user.UserId);

        Assert.AreEqual("jane", updatedUser.Name);
        Assert.AreEqual("jane@doe.com", updatedUser.Email);
    }
    
    [TestMethod]
    public void Update_ShouldReturnUpdatedUser()
    {
        User user = _userRepo.Add(new User("john", "john@doe.com", "testPass!1", true));

        user.Name = "jane";
        user.Email = "jane@doe.com";

        var updatedUser = _userRepo.Update(user);
        
        Assert.AreEqual("jane", updatedUser.Name);
        Assert.AreEqual("jane@doe.com", updatedUser.Email);
    }

    [TestMethod]
    public void GetByEmail_FoundUser_ShouldReturnUser()
    {
        User user = _userRepo.Add(new User("john", "john@doe.com", "testPass!1", true));

        var foundUser = _userRepo.Get("john@doe.com");

        Assert.AreEqual(user, foundUser);
    }

    [TestMethod]
    public void DeleteByEmail_ShouldDeleteUser()
    {
        User user = _userRepo.Add(new User("john", "john@doe.com", "testPass!1", true));

        _userRepo.Delete("john@doe.com");

        Assert.AreEqual(0, _userRepo.GetAll().Count);
    }

    [TestMethod]
    public void DeleteByEmail_NotFoundUser_ShouldNotDeleteUser()
    {
        _userRepo.Add(new User("john", "john@doe.com", "testPass!1", true));

        _userRepo.Delete("jane@doe.com");

        Assert.AreEqual(1, _userRepo.GetAll().Count);
    }
}
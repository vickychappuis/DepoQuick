using DepoQuick.Models;

namespace DepoQuick.Tests.Models;

[TestClass]
public class UserModel
{
    private string _name = "John Doe";
    private string _email = "validEmail@gmail.com";
    private string _password = "validPassword.1";
    private bool _isAdmin = false;
    
    [TestMethod]
    public void User_ShouldConstruct()
    {
        // Arrange
        var user = new User(_name, _email, _password, _isAdmin);
        
        // Act
        var name = user.Name;
        var email = user.Email;
        var password = user.Password;
        var isAdmin = user.IsAdmin;
        
        // Assert
        Assert.AreEqual(_name, name);
        Assert.AreEqual(_email, email);
        Assert.AreEqual(_password, password);
        Assert.AreEqual(_isAdmin, isAdmin);
    }
    
    [TestMethod]
    public void UserEquals_WithSameAttributes_ShouldReturnTrue()
    {
        var user1 = new User(_name, _email, _password, _isAdmin);
        var user2 = new User(_name, _email, _password, _isAdmin);
        
        Assert.IsTrue(user1.Equals(user2));
    }
    
    [TestMethod]
    public void UserEquals_WithDifferentAttributes_ShouldReturnFalse()
    {
        var user1 = new User(_name, _email, _password, _isAdmin);
        var user2 = new User("Jane Doe", "jane@doe.com", _password, false);
        
        Assert.IsFalse(user1.Equals(user2));
    }
    
    [TestMethod]
    public void UserEquals_WithNull_ShouldReturnFalse()
    {
        var user1 = new User(_name, _email, _password, _isAdmin);
        User? user2 = null;
        
        Assert.IsFalse(user1.Equals(user2));
    }
}
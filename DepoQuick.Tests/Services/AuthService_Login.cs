using DepoQuick.Backend.Dtos.Auth;
using DepoQuick.Models;
using DepoQuick.DataAccess.Repos;
using DepoQuick.Backend.Services;
using DepoQuick.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DepoQuick.Tests.Services
{
    [TestClass]
    public class AuthService_Login
    {
        private IRepo<User, string> _userRepo;
        private AuthService _authService;
        private readonly string _validName = "John Doe";
        private readonly string _validEmail = "example@email.com";
        private readonly string _validPassword = "Pass123#";

        [TestInitialize]
        public void TestInit()
        {
            var testsContext = new TestProgram();
            using var scope = testsContext.ServiceProvider.CreateScope();
            _userRepo = scope.ServiceProvider.GetRequiredService<IRepo<User, string>>();
            _authService = new AuthService(_userRepo);
        }
        
        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        [DataRow("  ")] // two white spaces
        [DataRow("\t")] // tab
        public void Login_NullOrWhiteSpaceEmail_ShouldThrow(string invalidEmail)
        {
            Assert.ThrowsException<ArgumentException>(() => _authService.Login(invalidEmail, _validPassword));
        }
        
        
        [TestMethod]
        public void Login_EmptyPassword_ShouldThrow()
        {
            const string password = "";
            Assert.ThrowsException<ArgumentException>(() => _authService.Login(_validEmail, password));
        }

        [TestMethod]
        public void Login_UserNotFound_ShouldThrow()
        {
            Assert.ThrowsException<ArgumentException>(() => _authService.Login(_validEmail, _validPassword));
        }
        
        [TestMethod]
        public void Login_PasswordDoesNotMatch_ShouldThrow()
        {
            const string password1 = "somePassword123#";
            const string password2 = "differentPassword123#";
            _authService.Signup( _validName, _validEmail, password1, password1);
            Assert.ThrowsException<ArgumentException>(() => _authService.Login(_validEmail, password2));
        }

        [TestMethod]
        public void Login_WithValidCredentials_ShouldReturnUser()
        {
            _authService.Signup( _validName, _validEmail, _validPassword, _validPassword);
            User user = _authService.Login(_validEmail, _validPassword);
            
            Assert.AreEqual(user.Name, _validName);
            Assert.AreEqual(user.Email, _validEmail);
            Assert.AreEqual(user.Password, _validPassword);
            Assert.AreEqual(user.IsAdmin, true);
            
            Assert.AreEqual(user, _authService.AuthedUser);
        }

        [TestMethod]
        public void Login_GivenDto_ShouldCallLogin()
        {
            LoginDto loginDto = new LoginDto
            {
                Email = _validEmail,
                Password = _validPassword,
            };
            
            _authService.Signup( _validName, _validEmail, _validPassword, _validPassword);
            User user = _authService.Login(loginDto);
            
            Assert.AreEqual(user.Name, _validName);
            Assert.AreEqual(user.Email, _validEmail);
            Assert.AreEqual(user.Password, _validPassword);
            Assert.AreEqual(user.IsAdmin, true);
            
            Assert.AreEqual(user, _authService.AuthedUser);
        }
    }
}

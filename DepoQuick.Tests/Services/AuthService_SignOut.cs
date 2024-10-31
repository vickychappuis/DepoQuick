using DepoQuick.Models;
using DepoQuick.DataAccess.Repos;
using DepoQuick.Backend.Services;
using DepoQuick.DataAccess;
using Microsoft.Extensions.DependencyInjection;

namespace DepoQuick.Tests.Services;

[TestClass]
public class AuthService_SignOut
{
        private IRepo<User, string> _userRepo;
        private AuthService _authService;

        [TestInitialize]
        public void TestInit()
        {
            var testsContext = new TestProgram();
            using var scope = testsContext.ServiceProvider.CreateScope();
            _userRepo = scope.ServiceProvider.GetRequiredService<IRepo<User, string>>();
            _authService = new AuthService(_userRepo);
        }
        
        [TestMethod]
        public void SignOut_UserIsNotLoggedIn_ShouldThrow()
        {
            Assert.ThrowsException<InvalidOperationException>(() => _authService.SignOut());
        }
        
        [TestMethod]
        public void SignOut_UserIsAuthed_ShouldSignOut()
        {
            _authService.AuthedUser = new User("John Doe", "john@doe.com", "TestPass1#", true);
            
            _authService.SignOut();
            
            Assert.IsNull(_authService.AuthedUser);
        }
}
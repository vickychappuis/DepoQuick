using DepoQuick.Backend.Dtos.Auth;
using DepoQuick.Models;
using DepoQuick.DataAccess.Repos;
using DepoQuick.Backend.Services;
using DepoQuick.DataAccess;
using Microsoft.Extensions.DependencyInjection;

namespace DepoQuick.Tests.Services
{
    [TestClass]
    public class AuthService_SignUp
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
        public void SignUp_NameLongerThan100_ShouldThrow()
        {
            string longName = new string('v', 101);

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => _authService.Signup(longName, _validEmail, _validPassword, _validPassword));
        }

        [TestMethod]
        public void Signup_EmailLongerThan254_ShouldThrow()
        {
            string longEmail = $"{new string('v', 255)}@domain.com";
                
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => _authService.Signup(_validName, longEmail, _validPassword, _validPassword));
        }
        
        [TestMethod]
        [DataRow("invalid")]
        [DataRow("invalid@")]
        [DataRow("invalid@.")]
        [DataRow("invalid@.com")]
        [DataRow("invalid@domain")]
        [DataRow("invalid@domain.")]
        [DataRow("@domain")]
        [DataRow("@domain.")]
        [DataRow("@domain.com")]
        [DataRow("@.com")]
        [DataRow(".com")]
        public void SignUp_EmailNotValid_ShouldThrow(string invalidEmail)
        {
            Assert.ThrowsException<ArgumentException>(() => _authService.Signup(_validName, invalidEmail, _validPassword, _validPassword));
        }

        [TestMethod]
        [DataRow("invalidPass1")]
        public void SignUp_PasswordNotContainsSymbol_ShouldThrow(string invalidPassword)
        {
            Assert.ThrowsException<ArgumentException>(() => _authService.Signup(_validName, _validEmail, invalidPassword, _validPassword));
        }

        [TestMethod]
        [DataRow("INVALIDPASS.1")]
        [DataRow("INVALIDPASS.1Ñ")]
        public void SignUp_PasswordNotContainsLowercase_ShouldThrow(string invalidPassword)
        {
            Assert.ThrowsException<ArgumentException>(()=> _authService.Signup(_validName, _validEmail, invalidPassword, _validPassword));
        }

        [TestMethod]
        [DataRow("iñvalidpass.1")]
        public void SignUp_PasswordNotContainsUppercase_ShouldThrow(string invalidPassword)
        {
            Assert.ThrowsException<ArgumentException>(()=> _authService.Signup(_validName, _validEmail, invalidPassword, _validPassword));
        }
        
        [TestMethod]
        [DataRow("Iñvalidpass.")]
        [DataRow("")]
        [DataRow(" ")]
        public void SignUp_PasswordNotContainsDigit_ShouldThrow(string invalidPassword)
        {
            Assert.ThrowsException<ArgumentException>(()=> _authService.Signup(_validName, _validEmail, invalidPassword, _validPassword));
        }

        [TestMethod]
        public void SignUp_ConfirmationNotEqualsPassword_ShouldThrow()
        {
            const string password = "validPass.1";
            const string invalidConfirmation = "invalidConfirmation!!!";
            
            Assert.ThrowsException<ArgumentException>(()=> _authService.Signup(_validName, _validEmail, password, invalidConfirmation));
        }
        
        [TestMethod]
        public void SignUp_FirstSignUp_ShouldBeAdmin()
        {
            // Sign up first user (admin)
            User admin = _authService.Signup(_validName, _validEmail, _validPassword, _validPassword);
            
            Assert.IsNotNull(admin);
            Assert.AreEqual(1, _userRepo.GetAll().Count);
            Assert.AreEqual(_validName, admin.Name);
            Assert.AreEqual(_validEmail, admin.Email);
            Assert.AreEqual(_validPassword, admin.Password);

            Assert.AreEqual(1, admin.UserId);
            Assert.AreEqual(true, admin.IsAdmin);
            
            Assert.AreEqual(admin, _authService.AuthedUser);
        }

        [TestMethod]
        public void SignUp_ShouldReturnUniqueIds()
        { 
            const string validEmail2 = "hello@myemail.com";
            
            User user1 = _authService.Signup(_validName, _validEmail, _validPassword, _validPassword);
           User user2 = _authService.Signup(_validName, validEmail2, _validPassword, _validPassword);
            
           Assert.AreNotEqual(user1.UserId, user2.UserId);
        }
        
        [TestMethod]
        public void AddWarehouse_ShouldAssignIdsIncrementally()
        {
            const string validEmail2 = "hello@myemail.com";
            User user1 = _authService.Signup(_validName, _validEmail, _validPassword, _validPassword);
            User user2 = _authService.Signup(_validName, validEmail2, _validPassword, _validPassword);

            Assert.AreEqual(1, user1.UserId);
            Assert.AreEqual(2, user2.UserId);
        }
        
        [TestMethod]
        public void SignUp_AfterFirstSignUp_ShouldBeClient() 
        {
            const string clientName = "Client";
            const string clientEmail = "client@domain.com";
            const string clientPassword = "ClientPass.1";
            
            // Sign up first user (admin)
            _authService.Signup( _validName, _validEmail, _validPassword, _validPassword);
            
            // Sign up second user (client)
            User client =  _authService.Signup(clientName, clientEmail, clientPassword, clientPassword);
            
            Assert.IsNotNull(client);
            Assert.AreEqual(2, _userRepo.GetAll().Count);
            Assert.AreEqual(clientName, client.Name);
            Assert.AreEqual(clientEmail, client.Email);
            Assert.AreEqual(clientPassword, client.Password);
            Assert.AreEqual(2, client.UserId);
            Assert.IsFalse(client.IsAdmin);
            
            Assert.AreEqual(client, _authService.AuthedUser);
        }
        
        [TestMethod]
        public void SignUp_ShouldAddUserToDatabase()
        {
            Assert.AreEqual(0, _userRepo.GetAll().Count);
            
            User user = _authService.Signup(_validName, _validEmail, _validPassword, _validPassword);
            
            Assert.AreEqual(1, _userRepo.GetAll().Count);
            Assert.AreEqual(user, _userRepo.GetAll()[0]);
        }
        
        [TestMethod]
        public void SignUp_CantHaveTwoUsersWithSameEmail_ShouldThrow()
        {
            const string email = "john@domain.com";

            const string secondUserName = "Marie Green";
            const string secondUserPassword = "MariePass.1";
            _authService.Signup(_validName, email, _validPassword, _validPassword);
            
            Assert.ThrowsException<ArgumentException>(()=> _authService.Signup(secondUserName, email, secondUserPassword, secondUserPassword));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        [DataRow("  ")] // Two white spaces
        [DataRow("\t")] // Tab
        public void SignUp_NullOrWhiteSpaceEmail_ShouldThrow(string invalidEmail)
        {
            Assert.ThrowsException<ArgumentException>(() =>
                _authService.Signup(_validName, invalidEmail, _validPassword, _validPassword));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        [DataRow("  ")] // Two white spaces
        [DataRow("\t")] // Tab
        public void SignUp_NullOrWhiteSpaceName_ShouldThrow(string invalidName)
        {
            Assert.ThrowsException<ArgumentException>(() =>
                _authService.Signup(invalidName, _validEmail, _validPassword, _validPassword));
        }
        
        [TestMethod]
        public void SignUp_GivenDto_ShouldCallSignUp()
        {
            SignUpDto signUpDto = new SignUpDto
            {
                Name = _validName,
                Email = _validEmail,
                Password = _validPassword,
                Confirmation = _validPassword
            };
            
            User user = _authService.Signup(signUpDto);
            
            Assert.AreEqual(_validName, user.Name);
            Assert.AreEqual(_validEmail, user.Email);
            Assert.AreEqual(_validPassword, user.Password);
            Assert.AreEqual(true, user.IsAdmin);
            
            Assert.AreEqual(1, _userRepo.GetAll().Count);
            Assert.AreEqual(user, _userRepo.GetAll()[0]);
            
            Assert.AreEqual(user, _authService.AuthedUser);
        }
        
        [TestMethod]
        public void AddUser_GivenDto_ShouldCallAddUser()
        {
            SignUpDto signUpDto = new SignUpDto
            {
                Name = _validName,
                Email = _validEmail,
                Password = _validPassword,
                Confirmation = _validPassword
            };
            
            User user = _authService.AddUser(signUpDto);
            
            Assert.IsNotNull(user);
            Assert.AreEqual(user.Name, _validName);
            Assert.AreEqual(user.Email, _validEmail);
            Assert.AreEqual(user.Password, _validPassword);
            Assert.AreEqual(user.IsAdmin, true);
        }
    }
}

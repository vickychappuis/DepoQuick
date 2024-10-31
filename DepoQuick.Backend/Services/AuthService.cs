using System.Text.RegularExpressions;
using DepoQuick.Backend.Dtos.Auth;
using DepoQuick.Models;
using DepoQuick.DataAccess.Repos;

namespace DepoQuick.Backend.Services;

public class AuthService
{
    public User? AuthedUser { get; set; }

    public bool AdminExists { get; private set; }

    // Injected
    private readonly IRepo<User, string> _userRepo;

    public AuthService(IRepo<User, string> userRepo)
    {
        _userRepo = userRepo;

        if (userRepo.GetAll().Any(u => u.IsAdmin))
            AdminExists = true;
    }

    private void CheckPassword(string password, string confirmation)
    {
        const string specialCharacter = @"[#@$.,%]";
        if (!Regex.IsMatch(password, specialCharacter))
            throw new ArgumentException("Password must contain a symbol (#@$.,%)", nameof(password));

        const string lowercase = @"[a-zñ]";
        if (!Regex.IsMatch(password, lowercase))
            throw new ArgumentException("Password must contain a lowercase letter.", nameof(password));

        const string uppercase = @"[A-ZÑ]";
        if (!Regex.IsMatch(password, uppercase))
            throw new ArgumentException("Password must contain an uppercase letter.", nameof(password));

        const string digit = @"[0-9]";
        if (!Regex.IsMatch(password, digit))
            throw new ArgumentException("Password must contain a digit.", nameof(password));

        if (password != confirmation)
            throw new ArgumentException("Confirmation should match password.", nameof(confirmation));
    }

    private void CheckName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.");

        const int maxNameLength = 100;

        if (name.Length > maxNameLength)
            throw new ArgumentOutOfRangeException(nameof(name));
    }

    private void CheckEmail(string? email)
    {
        string? parsedEmail = email?.Trim();

        if (string.IsNullOrWhiteSpace(parsedEmail))
            throw new ArgumentException("Email cannot be empty.");

        const int emailMaxLength = 254;
        if (parsedEmail.Length > emailMaxLength)
            throw new ArgumentOutOfRangeException(nameof(email));

        const string emailPattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";
        if (!Regex.IsMatch(parsedEmail, emailPattern))
            throw new ArgumentException("Email is formatted incorrectly.", nameof(email));
    }

    private User AddUser(string name, string email, string password, string confirmation)
    {
        CheckName(name);
        CheckEmail(email);

        if (_userRepo.GetAll().Any(u => u.Email == email))
            throw new ArgumentException("A user with this email already exists.", nameof(email));

        CheckPassword(password, confirmation);

        User newUser = new User(name, email, password, false); // Initialize User as a client

        // If there are no admins, add the user as an admin
        if (AdminExists == false)
        {
            newUser.IsAdmin = true; // Add User as an admin
            AdminExists = true;
        }

        _userRepo.Add(newUser);

        return newUser;
    }

    public User AddUser(SignUpDto signUpDto)
    {
        return AddUser(signUpDto.Name, signUpDto.Email, signUpDto.Password, signUpDto.Confirmation);
    }

    public User Signup(string name, string email, string password, string confirmation)
    {
        User newUser = AddUser(name, email, password, confirmation);

        AuthedUser = newUser;

        return newUser;
    }

    public User Signup(SignUpDto signUpDto)
    {
        return Signup(signUpDto.Name, signUpDto.Email, signUpDto.Password, signUpDto.Confirmation);
    }

    public User Login(string email, string password)
    {
        CheckEmail(email);

        if (string.IsNullOrEmpty(password))
            throw new ArgumentException("Password cannot be empty.", nameof(password));

        User? user = _userRepo.Get(email);

        if (user is null)
            throw new ArgumentException("User not found.", nameof(email));

        if (user.Password != password)
            throw new ArgumentException("Incorrect password.", nameof(password));

        AuthedUser = user;

        return user;
    }

    public User Login(LoginDto loginFormDto)
    {
        return Login(loginFormDto.Email, loginFormDto.Password);
    }

    public void SignOut()
    {
        if (AuthedUser is null)
            throw new InvalidOperationException("User is not logged in.");

        AuthedUser = null;
    }
}
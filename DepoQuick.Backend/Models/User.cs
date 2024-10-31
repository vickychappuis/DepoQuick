namespace DepoQuick.Backend.Models;

public class User
{
    public static int nextUserId = 0;
    public int Id { get; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public bool IsAdmin { get; set; }

    
    public User(string name, string email, string password, bool isAdmin)
    {
        Id = nextUserId++;
        Name = name;
        Email = email;
        Password = password;
        IsAdmin = isAdmin;
    }

}


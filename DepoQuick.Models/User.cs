using System.ComponentModel.DataAnnotations;

namespace DepoQuick.Models;

public class User
{
    [Key, Range(0, int.MaxValue)]
    public int UserId { get; init; }
    [Required,MaxLength(100)]
    public string Name { get; set; }
    [Required, EmailAddress]
    public string Email { get; set; }
    [Required]
    public string Password { get; init; }
    [Required]
    public bool IsAdmin { get; set; }
    
    public User(string name, string email, string password, bool isAdmin)
    {
        Name = name;
        Email = email;
        Password = password;
        IsAdmin = isAdmin;
    }

    public override bool Equals(object? obj)
    {
        var user = obj as User;
        if (user == null) return false;

        return Name == user.Name && Email == user.Email && Password == user.Password && IsAdmin == user.IsAdmin;
    }
}


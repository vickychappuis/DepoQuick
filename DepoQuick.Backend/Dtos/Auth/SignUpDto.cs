using System.ComponentModel.DataAnnotations;

namespace DepoQuick.Backend.Dtos.Auth;

public class SignUpDto
{
    [Required, MaxLength(100, ErrorMessage = "Name length shouldn't exceed 100 characters.")]
    public string Name { get; set; }
    
    [Required, EmailAddress]
    public string Email { get; set; }
    
    [Required]
    public string Password { get; set; }
    
    [Required, Compare(nameof(Password), ErrorMessage = "Confirmation should match password.")]
    public string Confirmation { get; set; }
}
using System.ComponentModel.DataAnnotations;

namespace BookTracker.Blazor.Models.Auth;

public sealed class LoginRequest
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Enter a valid email address.")]

    public string Email { get; set; } = string.Empty;
    [Required(ErrorMessage = "Password is required.")]
    [MinLength(8, ErrorMessage = "Password needs to be 8 characters long.")]
    public string Password { get; set; } = string.Empty;
}
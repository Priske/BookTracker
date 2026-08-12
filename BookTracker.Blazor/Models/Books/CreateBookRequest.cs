using System.ComponentModel.DataAnnotations;

namespace BookTracker.Blazor.Models.Books;

public sealed class CreateBookRequest
{
    [Required(ErrorMessage = "Title is required.")]
    public string Title { get; set; } = string.Empty;
    [Required(ErrorMessage = "Author is Required.")]
    public string Author { get; set; } = string.Empty;

    public int Year { get; set; }
}
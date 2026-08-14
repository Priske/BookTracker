using System.ComponentModel.DataAnnotations;

namespace BookTracker.Blazor.Models.Books;

public class UpdateBookRequest
{
    [Required(ErrorMessage = "Title is required.")]
    public required string Title { get; set; }
    [Required(ErrorMessage = "Author is Required.")]
    public required string Author { get; set; }
    public int Year { get; set; }

    public Guid Version { get; set; }
}




using System.ComponentModel.DataAnnotations;

namespace FinalProject.Models;

// DTO class for validating and transferring product data including an optional image file.
public class ProductDto
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = "";

    [Required, MaxLength(100)]
    public string Brand { get; set; } = "";

    [Required, MaxLength(100)]
    public string Category { get; set; } = "";

    [Required]
    public decimal Price { get; set; }

    [Required]
    public string Description { get; set; } = "";

    public IFormFile? ImageFile { get; set; }
}

using System.ComponentModel.DataAnnotations;

namespace DepoQuick.Backend.Dtos.Promotions;

public class AddPromotionDto
{
    [Required]
    public string Label { get; set; }
    
    [Required]
    public int DiscountPercentage { get; set; }
    
    [Required]
    public DateTime StartDate { get; set; }
    
    [Required]
    public DateTime EndDate { get; set; }
}
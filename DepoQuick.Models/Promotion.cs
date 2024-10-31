using System.ComponentModel.DataAnnotations;


namespace DepoQuick.Models;

public class Promotion
{
    [Key, Range(0, int.MaxValue)]
    public int PromotionId { get; set; }
    [Required, Range(0, 100)]
    private int _discountPercentage;
    [Required, MaxLength(20)]
    public string Label { get; set; }

    public int DiscountPercentage
    {
        get => _discountPercentage;
        set
        {
            const int maxDiscount = 100;
            const int minDiscount = 0;
            
            if (value > maxDiscount || value < minDiscount)
                throw new ArgumentOutOfRangeException(nameof(value));
            _discountPercentage = value;
        }
    }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    
    public bool IsUsed { get; set; }
 
     public Promotion(string label, int discountPercentage, DateTime startDate, DateTime endDate)
     {
         if (startDate > endDate)
             throw new ArgumentException(String.Format("End date ({0}) can't be larger than start date ({1})", endDate.ToShortDateString(), startDate.ToShortDateString()), nameof(endDate));
         
         Label = label;
         DiscountPercentage = discountPercentage;
         StartDate = startDate;
         EndDate = endDate;
         IsUsed = false;
     }

     public override bool Equals(object? obj)
     {
            var promotion = obj as Promotion;
            if (promotion == null) return false;
    
            return Label == promotion.Label && DiscountPercentage == promotion.DiscountPercentage && StartDate == promotion.StartDate && EndDate == promotion.EndDate && IsUsed == promotion.IsUsed;
     }
}
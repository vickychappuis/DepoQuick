namespace DepoQuick.Backend.Models;

public class Promotion
{
    public static int NextPromotionId = 0;

    private int _discountPercentage;
    
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
    public int Id { get; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    
    public bool IsUsed { get; set; }
 
     public Promotion(string label, int discountPercentage, DateTime startDate, DateTime endDate)
     {
         if (startDate > endDate)
             throw new ArgumentException(String.Format("End date ({0}) can't be larger than start date ({1})", endDate.ToShortDateString(), startDate.ToShortDateString()), nameof(endDate));
         
         Id = NextPromotionId++;
         Label = label;
         DiscountPercentage = discountPercentage;
         StartDate = startDate;
         EndDate = endDate;
         IsUsed = false;
     }
}
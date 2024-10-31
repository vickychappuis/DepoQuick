using DepoQuick.DataAccess.Repos;
using DepoQuick.Models;

namespace DepoQuick.Backend.Services;
public class PriceService
{
    
    private const int SmallDiscount = 50;
    private const int MediumDiscount = 75;
    private const int LargeDiscount = 100;
    
    private const int HeatedDiscountPercentage = 20;
    private const int UnheatedDiscountPercentage = 0;
    
    private readonly IRepo<Promotion, int> _promotionRepo;
    
    public PriceService(IRepo<Promotion, int> promotionRepo)
    {
        _promotionRepo = promotionRepo;
    }
    private double CalculatePriceBySize(WarehouseSize size, double stayInDays)
    {
        return size switch
        {
            WarehouseSize.Small => SmallDiscount * stayInDays,
            WarehouseSize.Medium => MediumDiscount * stayInDays,
            WarehouseSize.Large => LargeDiscount * stayInDays,
            _ => throw new ArgumentException("Invalid warehouse size: " + size)
        };
    }
    
    private double CalculateDiscountMultiplierByReservationStay(double stayInDays)
    {
        return stayInDays switch
        {
            < 7 => 1,
            <= 14 => 0.95,
            > 14 => 0.90, 
        };
    }
    
    private double CalculateTotalDiscountForAvailablePromotions(DateTime startDate)
    {
        double totalDiscount = 0;
        var promotions = _promotionRepo.GetAll();

        foreach (var promotion in promotions)
        {
            if (promotion.StartDate <= startDate && promotion.EndDate >= startDate)
            {
                totalDiscount += promotion.DiscountPercentage;
            }
        }
        
        const double maxTotalDiscount = 100;
        return Math.Min(totalDiscount, maxTotalDiscount);
    }


    public double CalculatePrice(WarehouseSize warehouseSize, bool warehouseIsHeated, DateTime startDate, DateTime endDate)
    {
         double stayInDays = (endDate - startDate).TotalDays;

         double sizePrice = CalculatePriceBySize(warehouseSize, stayInDays);
         double stayDiscount = CalculateDiscountMultiplierByReservationStay(stayInDays);
         double heatedPrice = warehouseIsHeated ? HeatedDiscountPercentage * stayInDays : UnheatedDiscountPercentage;
         double promotionDiscounts = 1 - (CalculateTotalDiscountForAvailablePromotions(startDate) / 100);  // 1 - (discount / 100) to get the discount multiplier
         double totalPrice = (sizePrice + heatedPrice) * promotionDiscounts * stayDiscount;
         return totalPrice;
    }
    
} 

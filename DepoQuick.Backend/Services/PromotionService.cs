using DepoQuick.Backend.Dtos.Promotions;
using DepoQuick.Models;
using DepoQuick.DataAccess.Repos;

namespace DepoQuick.Backend.Services;

public class PromotionService
{
    private readonly IRepo<Promotion, int> _promotionRepo;
    
    public PromotionService(IRepo<Promotion, int> promotionRepo)
    {
        _promotionRepo = promotionRepo;
    }
    
    private void AssertPromotionIsValid(Promotion promotion)
    {
        const int maxLabelLength = 20;
        const int maxDiscountPercentage = 75;
        const int minDiscountPercentage = 5;
        
        if (promotion.Label.Length > maxLabelLength)
            throw new ArgumentOutOfRangeException(nameof(promotion.Label), $"Label can't be longer than {maxLabelLength} characters");
        if (promotion.DiscountPercentage > maxDiscountPercentage || promotion.DiscountPercentage < minDiscountPercentage)
            throw new ArgumentOutOfRangeException(nameof(promotion.DiscountPercentage), $"Discount percentage must be between {minDiscountPercentage} and {maxDiscountPercentage}");
    }

    
    public Promotion AddPromotion(string label, int discountPercentage, DateTime startDate, DateTime endDate)
    {
        Promotion newPromotion = new Promotion(label, discountPercentage, startDate, endDate);

        AssertPromotionIsValid(newPromotion);
        
        _promotionRepo.Add(newPromotion);
        
        return newPromotion;
    }
    
    public Promotion AddPromotion(AddPromotionDto addPromotionDto)
    {
        return AddPromotion(addPromotionDto.Label, addPromotionDto.DiscountPercentage, addPromotionDto.StartDate, addPromotionDto.EndDate);
    }

    public List<Promotion> GetAll()
    {
        return _promotionRepo.GetAll();
    }
    
    public void DeletePromotion(int promotionId)
    {
        var promotion = _promotionRepo.Get(promotionId);
        if (promotion != null && promotion.IsUsed)
            throw new InvalidOperationException("Can't delete a promotion that has been used");
        _promotionRepo.Delete(promotionId);
    }

    public Promotion? Get(int promotionId)
    {
        return _promotionRepo.Get(promotionId);
    }

    public Promotion UpdatePromotion(int id, AddPromotionDto addPromotionFormDto)
    {
        var updatedPromotion = new Promotion(addPromotionFormDto.Label, addPromotionFormDto.DiscountPercentage, addPromotionFormDto.StartDate, addPromotionFormDto.EndDate) {PromotionId = id};
        
        AssertPromotionIsValid(updatedPromotion);
        
        return _promotionRepo.Update(updatedPromotion);
    }
}
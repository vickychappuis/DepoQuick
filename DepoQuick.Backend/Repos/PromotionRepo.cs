using DepoQuick.Backend.Dtos.Promotions;
using DepoQuick.Backend.Models;
using DepoQuick.Backend.Providers;

namespace DepoQuick.Backend.Repos;

public class PromotionRepo : IRepo<Promotion>
{
    private InMemoryDatabase _db;

    public PromotionRepo(InMemoryDatabase database)
    {
        _db = database;
    }
    
    public void Add(Promotion promotion)
    {
        _db.Promotions.Add(promotion);
    }
    
    public List<Promotion> GetAll()
    {
        return _db.Promotions;
    }
    
    public Promotion? Get(int id)
    {
        return _db.Promotions.Find(u => u.Id == id);
    }
    
    public void Delete(int id)
    {
        var promotion = Get(id);
        _db.Promotions.Remove(promotion);
    }

    public void UpdatePromotion(int id, AddPromotionDto addPromotionFormDto)
    {
        var promotion = Get(id);
        
        promotion.Label = addPromotionFormDto.Label;
        promotion.DiscountPercentage = addPromotionFormDto.DiscountPercentage;
        promotion.StartDate = addPromotionFormDto.StartDate;
        promotion.EndDate = addPromotionFormDto.EndDate;
    }
}
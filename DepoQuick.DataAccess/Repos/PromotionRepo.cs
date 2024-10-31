using DepoQuick.Models;
using Microsoft.EntityFrameworkCore;

namespace DepoQuick.DataAccess.Repos;

public class PromotionRepo : IRepo<Promotion, int>
{
    private readonly IDbContextFactory<Context> _contextFactory;

    public PromotionRepo(IDbContextFactory<Context> context)
    {
        _contextFactory = context;
    }
    
    public Promotion Add(Promotion promotion)
    {
        using var context = _contextFactory.CreateDbContext();
        var promotionEntry = context.Promotions.Add(promotion);
        context.SaveChanges();
        
        return promotionEntry.Entity;
    }
    
    public List<Promotion> GetAll()
    {
        using var context = _contextFactory.CreateDbContext();
        return context.Promotions.ToList();
    }
    
    public Promotion? Get(int id)
    {
        using var context = _contextFactory.CreateDbContext();
        return context.Promotions.Find(id);
    }
    
    public void Delete(int id)
    {
        using var context = _contextFactory.CreateDbContext();
        var promotion = context.Promotions.Find(id);
        
        if (promotion is null)
            return;
        
        context.Promotions.Remove(promotion);
        context.SaveChanges();
    }

    public Promotion Update(Promotion entity)
    {
        using var context = _contextFactory.CreateDbContext();
        var promotionEntry = context.Promotions.Update(entity);
        context.SaveChanges();
        
        return promotionEntry.Entity;
    }
}
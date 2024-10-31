using DepoQuick.DataAccess;
using DepoQuick.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DepoQuick.Tests.Repos;

[TestClass]
public class PromotionRepo
{
    private IDbContextFactory<Context> _contextFactory;
    private DataAccess.Repos.PromotionRepo _promotionRepo;

    [TestInitialize]
    public void TestInit()
    {
        var testsContext = new TestProgram();
        using var scope = testsContext.ServiceProvider.CreateScope();
        _contextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<Context>>();
        _promotionRepo = new DataAccess.Repos.PromotionRepo(_contextFactory);
    }

    [TestMethod]
    public void Add_ShouldAddPromotion()
    {
        Promotion promotion =
            _promotionRepo.Add(new Promotion("Promotion 1", 25, new DateTime(2022, 1, 1), new DateTime(2022, 1, 31)));
        
        Assert.AreEqual(1, promotion.PromotionId);
        Assert.AreEqual(promotion, _promotionRepo.GetAll()[0]);
    }

    [TestMethod]
    public void Delete_ShouldDeletePromotion()
    {
        Promotion promotion =
            _promotionRepo.Add(new Promotion("Promotion 1", 25, new DateTime(2022, 1, 1), new DateTime(2022, 1, 31)));

        _promotionRepo.Delete(promotion.PromotionId);

        Assert.AreEqual(0, _promotionRepo.GetAll().Count);
    }
    
    [TestMethod]
    public void Delete_NotFoundPromotion_ShouldNotDeletePromotion()
    {
        Promotion promotion =
            _promotionRepo.Add(new Promotion("Promotion 1", 25, new DateTime(2022, 1, 1), new DateTime(2022, 1, 31)));

        _promotionRepo.Delete(promotion.PromotionId + 1);

        Assert.AreEqual(1, _promotionRepo.GetAll().Count);
    }
    
    [TestMethod]
    public void GetAll_ShouldReturnAllPromotions()
    {
        Promotion promotion1 =
            _promotionRepo.Add(new Promotion("Promotion 1", 25, new DateTime(2022, 1, 1), new DateTime(2022, 1, 31)));
        Promotion promotion2 =
            _promotionRepo.Add(new Promotion("Promotion 2", 50, new DateTime(2022, 2, 1), new DateTime(2022, 2, 28)));
        
        var promotions = _promotionRepo.GetAll();
        
        Assert.AreEqual(2, promotions.Count);
        Assert.AreEqual(promotion1, promotions[0]);
        Assert.AreEqual(promotion2, promotions[1]);
       }

    [TestMethod]
    public void Get_FoundPromotion_ShouldReturnPromotion()
    {
        Promotion promotion =
            _promotionRepo.Add(new Promotion("Promotion 1", 25, new DateTime(2022, 1, 1), new DateTime(2022, 1, 31)));

        Assert.AreEqual(promotion, _promotionRepo.Get(promotion.PromotionId));
    }

    [TestMethod]
    public void Get_NotFoundPromotion_ShouldReturnNull()
    {
        Assert.IsNull(_promotionRepo.Get(1));
    }

}
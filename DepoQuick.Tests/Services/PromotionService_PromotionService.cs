using DepoQuick.Backend.Dtos.Promotions;
using DepoQuick.Models;
using DepoQuick.Backend.Services;
using DepoQuick.DataAccess.Repos;
using DepoQuick.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DepoQuick.Tests.Services;

[TestClass]
public class PromotionService_PromotionService
{
    private Context _dbContext;
    private IRepo<Promotion, int> _promotionRepo;
    private PromotionService _promotionService;
    private readonly string _validLabel = "Some Promotion";
    private readonly int _validDiscountPercentage = 55;
    private readonly DateTime _validStartDate = DateTime.Parse("2024-01-01");
    private readonly DateTime _validEndDate = DateTime.Parse("2024-01-08");
    
    [TestInitialize]
    public void TestInit()
    {
        var testsContext = new TestProgram();
        using var scope = testsContext.ServiceProvider.CreateScope();
        _dbContext = scope.ServiceProvider.GetRequiredService<IDbContextFactory<Context>>().CreateDbContext();
        _promotionRepo = scope.ServiceProvider.GetRequiredService<IRepo<Promotion, int>>();
        _promotionService = new PromotionService(_promotionRepo);
    }

    [TestMethod]
    public void AddPromotion_ShouldAddPromotionToDatabase()
    {
        Assert.AreEqual(0, _promotionRepo.GetAll().Count);
       
        Promotion promotion =
            _promotionService.AddPromotion(_validLabel, _validDiscountPercentage, _validStartDate, _validEndDate);
        
        Assert.AreEqual(1,promotion.PromotionId);
        Assert.AreEqual(1, _promotionRepo.GetAll().Count);
        Assert.AreEqual(promotion, _promotionRepo.GetAll()[0]);
    }
    
    [TestMethod]
    public void AddPromotion_ShouldIncrementPromotionId()
    {
        Promotion promotion0 =
            _promotionService.AddPromotion(_validLabel, _validDiscountPercentage, _validStartDate, _validEndDate);
        Promotion promotion1 =
            _promotionService.AddPromotion(_validLabel, _validDiscountPercentage, _validStartDate, _validEndDate);
        Promotion promotion2 =
            _promotionService.AddPromotion(_validLabel, _validDiscountPercentage, _validStartDate, _validEndDate);
        
        Assert.AreEqual(1, promotion0.PromotionId);
        Assert.AreEqual(2, promotion1.PromotionId);
        Assert.AreEqual(3, promotion2.PromotionId);
    }

    [TestMethod]
    public void AddPromotion_ShouldReturnPromotion()
    {
        Promotion promotion =
            _promotionService.AddPromotion(_validLabel, _validDiscountPercentage, _validStartDate, _validEndDate);

        Assert.IsNotNull(promotion);
        Assert.AreEqual(promotion.Label, _validLabel);
        Assert.AreEqual(promotion.DiscountPercentage, _validDiscountPercentage);
        Assert.AreEqual(promotion.StartDate, _validStartDate);
        Assert.AreEqual(promotion.EndDate, _validEndDate);
    }

    [TestMethod]
    public void AddPromotion_DiscountLargerThan75_ShouldThrow()
    {
        int bigDiscountPercentage = 76;

        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            _promotionService.AddPromotion(_validLabel, bigDiscountPercentage, _validStartDate, _validEndDate));
    }

    [TestMethod]
    public void AddPromotion_DiscountSmallerThan5_ShouldThrow()
    {
        int smallDiscountPercentage = 4;

        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            _promotionService.AddPromotion(_validLabel, smallDiscountPercentage, _validStartDate, _validEndDate));
    }

    [TestMethod]
    public void AddPromotion_LabelLongerThan20_ShouldThrow()
    {
        string longLabel = new string('v', 21);

        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            _promotionService.AddPromotion(longLabel, _validDiscountPercentage, _validStartDate, _validEndDate));
    }
    
    [TestMethod]
    public void AddPromotion_StartDateAfterEndDate_ShouldThrow()
    {
        DateTime beforeDate = DateTime.Parse("2024-01-03");
        DateTime afterDate = DateTime.Parse("2024-01-08");

        Assert.ThrowsException<ArgumentException>(() =>
            _promotionService.AddPromotion(_validLabel, _validDiscountPercentage, afterDate, beforeDate));
    }
    
    [TestMethod]
    public void DeletePromotion_shouldDeletePromotion()
    {
        Promotion promotion =
            _promotionService.AddPromotion(_validLabel, _validDiscountPercentage, _validStartDate, _validEndDate);
        
        Assert.AreEqual(1, _promotionRepo.GetAll().Count);
        
        _promotionService.DeletePromotion(promotion.PromotionId);
        
        Assert.AreEqual(0, _promotionRepo.GetAll().Count);
    }

    [TestMethod]
    public void DeletePromotion_UsedPromotion_ShouldThrow()
    {
        Promotion promotion =
            _promotionService.AddPromotion(_validLabel, _validDiscountPercentage, _validStartDate, _validEndDate);

        promotion.IsUsed = true;
        _dbContext.Promotions.Update(promotion);
        _dbContext.SaveChanges();
        
        Assert.ThrowsException<InvalidOperationException>(() => _promotionService.DeletePromotion(promotion.PromotionId));
    }
    
    [TestMethod]
    public void UpdatePromotion_ShouldUpdatePromotion()
    {
        Promotion promotion =
            _promotionService.AddPromotion(_validLabel, _validDiscountPercentage, _validStartDate, _validEndDate);
        
        string newLabel = "New Label";
        int newDiscountPercentage = 10;
        DateTime newStartDate = DateTime.Parse("2024-01-02");
        DateTime newEndDate = DateTime.Parse("2024-01-09");
        
        var dto = new AddPromotionDto
        {
            Label = newLabel,
            DiscountPercentage = newDiscountPercentage,
            StartDate = newStartDate,
            EndDate = newEndDate
        };
        
        var updatedPromotion = _promotionService.UpdatePromotion(promotion.PromotionId, dto);
        
        Assert.AreEqual(newLabel, updatedPromotion.Label);
        Assert.AreEqual(newDiscountPercentage, updatedPromotion.DiscountPercentage);
        Assert.AreEqual(newStartDate, updatedPromotion.StartDate);
        Assert.AreEqual(newEndDate, updatedPromotion.EndDate);
    }
    
    [TestMethod]
    public void AddPromotion_GivenValidDto_ShouldCallAddPromotion()
    {
        var dto = new AddPromotionDto
        {
            Label = _validLabel,
            DiscountPercentage = _validDiscountPercentage,
            StartDate = _validStartDate,
            EndDate = _validEndDate
        };
            
        var promotion = _promotionService.AddPromotion(dto);
            
        Assert.AreEqual(1, _promotionRepo.GetAll().Count);
        Assert.AreEqual(promotion, _promotionRepo.GetAll()[0]);
    }
    
    [TestMethod]
    public void GetAll_ShouldReturnAllPromotions()
    {
        var promotion = _promotionService.AddPromotion(_validLabel, _validDiscountPercentage,_validStartDate, _validEndDate);
            
        var allPromotions = _promotionService.GetAll();
            
        Assert.AreEqual(1, allPromotions.Count);
        Assert.AreEqual(promotion, allPromotions[0]);
    }
    
    [TestMethod]
    public void Get_ShouldReturnPromotion()
    {
        var promotion = _promotionService.AddPromotion(_validLabel, _validDiscountPercentage,_validStartDate, _validEndDate);
            
        var foundPromotion = _promotionService.Get(promotion.PromotionId);
            
        Assert.AreEqual(promotion, foundPromotion);
    }
}
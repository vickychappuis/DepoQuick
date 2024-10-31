using DepoQuick.Backend.Dtos.Promotions;
using DepoQuick.Backend.Models;
using DepoQuick.Backend.Providers;
using DepoQuick.Backend.Repos;
using DepoQuick.Backend.Services;

namespace DepoQuick.Tests.Services;

[TestClass]
public class PromotionService
{
    private PromotionRepo _promotionRepo;
    private Backend.Services.PromotionService _promotionService;
    private InMemoryDatabase _db;
    private readonly string _validLabel = "Some Promotion";
    private readonly int _validDiscountPercentage = 55;
    private readonly DateTime _validStartDate = DateTime.Parse("2024-01-01");
    private readonly DateTime _validEndDate = DateTime.Parse("2024-01-08");
    
    [TestInitialize]
    public void TestInit()
    {
        _db = new InMemoryDatabase();
        _promotionRepo = new PromotionRepo(_db);
        _promotionService = new Backend.Services.PromotionService(_promotionRepo);
 
        // Reset NextPromotionId
        Promotion.NextPromotionId = 0;
    }

    [TestMethod]
    public void AddPromotion_ShouldAddPromotionToDatabase()
    {
        Assert.AreEqual(0, _db.Promotions.Count);
       
        Promotion promotion =
            _promotionService.AddPromotion(_validLabel, _validDiscountPercentage, _validStartDate, _validEndDate);
        
        Assert.AreEqual(0,promotion.Id);
        Assert.AreEqual(1, _db.Promotions.Count);
        Assert.AreSame(promotion, _db.Promotions[0]);
    }
    
    [TestMethod]
    public void AddPromotion_ShouldSetNextPromotionId()
    {
        Assert.AreEqual(0, Promotion.NextPromotionId);
        
        Promotion promotion =
            _promotionService.AddPromotion(_validLabel, _validDiscountPercentage, _validStartDate, _validEndDate);
        
        Assert.AreEqual(1, Promotion.NextPromotionId);
        Assert.AreEqual(0, promotion.Id);
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
        
        Assert.AreEqual(1, _db.Promotions.Count);
        
        _promotionService.DeletePromotion(promotion.Id);
        
        Assert.AreEqual(0, _db.Promotions.Count);
    }

    [TestMethod]
    public void DeletePromotion_UsedPromotion_ShouldThrow()
    {
        Promotion promotion =
            _promotionService.AddPromotion(_validLabel, _validDiscountPercentage, _validStartDate, _validEndDate);
        
        promotion.IsUsed = true;
        
        Assert.ThrowsException<InvalidOperationException>(() => _promotionService.DeletePromotion(promotion.Id));
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
        
        _promotionService.UpdatePromotion(promotion.Id, dto);
        
        Assert.AreEqual(newLabel, promotion.Label);
        Assert.AreEqual(newDiscountPercentage, promotion.DiscountPercentage);
        Assert.AreEqual(newStartDate, promotion.StartDate);
        Assert.AreEqual(newEndDate, promotion.EndDate);
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
            
        var foundPromotion = _promotionService.Get(promotion.Id);
            
        Assert.AreEqual(promotion, foundPromotion);
    }
}
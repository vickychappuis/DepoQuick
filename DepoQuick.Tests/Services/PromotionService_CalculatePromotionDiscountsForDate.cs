using DepoQuick.Backend.Models;
using DepoQuick.Backend.Providers;
using DepoQuick.Backend.Repos;
using DepoQuick.Backend.Services;

namespace DepoQuick.Tests.Services;

[TestClass]
public class PromotionService_CalculatePromotionDiscountsForDate
{
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
        var promotionRepo = new PromotionRepo(_db);
        _promotionService = new Backend.Services.PromotionService(promotionRepo);
    }

    [TestMethod]
    public void CalculatePromotionDiscountsForDate_NoPromotions_ShouldReturnEmpty()
    {
        double promotionPercentage = _promotionService.CalculatePromotionDiscountsForDate(_validStartDate);
        Assert.AreEqual(0, promotionPercentage);
    }

    [TestMethod]
    public void CalculatePromotionDiscountsForDate_OnePromotion_ShouldReturnPromotion()
    {
        Promotion promotion =
            _promotionService.AddPromotion(_validLabel, _validDiscountPercentage, _validStartDate, _validEndDate);

        double promotionPercentage = _promotionService.CalculatePromotionDiscountsForDate(_validStartDate);

        Assert.AreEqual(promotionPercentage, _validDiscountPercentage);
    }

    [TestMethod]
    public void CalculatePromotionDiscountsForDate_TwoPromotionShouldReturnAdditionOfTwoPromotions()
    {
        const string label1 = "Promotion 1 label";
        const string label2 = "Promotion 2 label";
        const int discountPercentage1 = 10;
        const int discountPercentage2 = 20;
        DateTime startDate1 = new DateTime(2024, 01, 01);
        DateTime startDate2 = new DateTime(2024, 01, 03);
        DateTime endDate1 = new DateTime(2024, 01, 05);
        DateTime endDate2 = new DateTime(2024, 01, 07);


        Promotion promotion1 = _promotionService.AddPromotion(label1, discountPercentage1, startDate1, endDate1);
        Promotion promotion2 = _promotionService.AddPromotion(label2, discountPercentage2, startDate2, endDate2);

        DateTime date = new DateTime(2024, 01, 04);

        double promotionPercentage = _promotionService.CalculatePromotionDiscountsForDate(date);

        Assert.AreEqual(30, promotionPercentage);
    }

    
    [TestMethod]
    public void CalculatePromotionDiscountsForDate_PromotionOutOfDateShouldReturnCero()
    {
        DateTime startDateOutOfDate = new DateTime(2023, 01, 01);
        DateTime endDateOutOfDate = new DateTime(2023, 01, 05);
        Promotion promotion = _promotionService.AddPromotion(_validLabel, _validDiscountPercentage, startDateOutOfDate, endDateOutOfDate);
        DateTime date = new DateTime(2024, 01, 04);
        
        double promotionPercentage = _promotionService.CalculatePromotionDiscountsForDate(date);
        
        Assert.AreEqual(0, promotionPercentage);
    }
    
    [TestMethod]
    public void CalculatePromotionDiscountsForDate_ShouldNotReturnMoreThan100()
    {
        const int discountpercentage1 = 70;
        const int discountpercentage2 = 70;
        Promotion promotion1 = _promotionService.AddPromotion(_validLabel, discountpercentage1, _validStartDate, _validEndDate);
        Promotion promotion2 = _promotionService.AddPromotion(_validLabel, discountpercentage2, _validStartDate, _validEndDate);

        DateTime date = new DateTime(2024, 01, 04);
        
        double promotionPercentage = _promotionService.CalculatePromotionDiscountsForDate(date);
        
        Assert.AreEqual(100, promotionPercentage);
    }
}
using DepoQuick.Models;

namespace DepoQuick.Tests.Models;

[TestClass]
public class PromotionModel
{
    private readonly string _validLabel = "Some Promotion";
    private readonly int _validDiscountPercentage = 90;
    private readonly DateTime _validStartDate = DateTime.Parse("2024-01-01");
    private readonly DateTime _validEndDate = DateTime.Parse("2024-01-08");
    
    [TestMethod]
    public void Promotion_ShouldConstructPromotion()
    {
        Promotion promotion = new Promotion(_validLabel, _validDiscountPercentage, _validStartDate, _validEndDate);

        Assert.AreEqual(promotion.Label, _validLabel);
        Assert.AreEqual(promotion.DiscountPercentage, _validDiscountPercentage);
        Assert.AreEqual(promotion.StartDate, _validStartDate);
        Assert.AreEqual(promotion.EndDate, _validEndDate);
        Assert.IsFalse(promotion.IsUsed);
    }

    [TestMethod]
    [DataRow(101)]
    [DataRow(200)]
    [DataRow(1000)]
    public void Promotion_DiscountLargerThan100_ShouldThrow(int invalidDiscountPercentage)
    {
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            new Promotion(_validLabel, invalidDiscountPercentage, _validStartDate, _validEndDate));
    }
    
    [TestMethod]
    [DataRow((int) -1)]
    [DataRow((int) -10)]
    public void Promotion_DiscountSmallerThan0_ShouldThrow(int invalidDiscountPercentage)
    {
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            new Promotion(_validLabel, invalidDiscountPercentage, _validStartDate, _validEndDate));
    }

    [TestMethod]
    public void Promotion_StartDateAfterEndDate_ShouldThrow()
    {
        DateTime beforeDate = DateTime.Parse("2024-01-03");
        DateTime afterDate = DateTime.Parse("2024-01-08");
        
        Assert.ThrowsException<ArgumentException>(() =>
            new Promotion(_validLabel, _validDiscountPercentage, afterDate, beforeDate));
    }
    
    [TestMethod]
    public void PromotionEquals_WithSameAttributes_ShouldReturnTrue()
    {
        Promotion promotion1 = new Promotion(_validLabel, _validDiscountPercentage, _validStartDate, _validEndDate);
        Promotion promotion2 = new Promotion(_validLabel, _validDiscountPercentage, _validStartDate, _validEndDate);
        
        Assert.IsTrue(promotion1.Equals(promotion2));
    }
    
    [TestMethod]
    public void PromotionEquals_WithDifferentAttributes_ShouldReturnFalse()
    {
        Promotion promotion1 = new Promotion(_validLabel, _validDiscountPercentage, _validStartDate, _validEndDate);
        Promotion promotion2 = new Promotion("Another Promotion", 50, DateTime.Parse("2024-01-01"), DateTime.Parse("2024-01-10"));
        
        Assert.IsFalse(promotion1.Equals(promotion2));
    }
    
    [TestMethod]
    public void PromotionEquals_WithNull_ShouldReturnFalse()
    {
        Promotion promotion1 = new Promotion(_validLabel, _validDiscountPercentage, _validStartDate, _validEndDate);
        Promotion? promotion2 = null;
        
        Assert.IsFalse(promotion1.Equals(promotion2));
    }
}
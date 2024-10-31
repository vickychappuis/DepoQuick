using DepoQuick.Models;
using DepoQuick.DataAccess.Repos;
using DepoQuick.Backend.Services;
using DepoQuick.DataAccess;
using Microsoft.Extensions.DependencyInjection;

namespace DepoQuick.Tests.Services;

[TestClass]
public class PriceService_CalculatePrice
{
    private IRepo<Promotion, int> _promotionRepo;
    private PriceService _priceService;
    
    [TestInitialize]
    public void TestInit()
    {
        var testsContext = new TestProgram();
        using var scope = testsContext.ServiceProvider.CreateScope();
        _promotionRepo = scope.ServiceProvider.GetRequiredService<IRepo<Promotion, int>>();
        _priceService = new PriceService(_promotionRepo);
    }

    [TestMethod]
    [DataRow(WarehouseSize.Small, false, 50)]
    [DataRow(WarehouseSize.Medium, false, 75)]
    [DataRow(WarehouseSize.Large, false, 100)]
    public void CalculatePrice_ForEveryWarehouseSize_ShouldReturnExpectedPrice(WarehouseSize size, bool isHeated, float expectedPrice)
    {
        Warehouse? warehouse = new Warehouse("Test Warehouse", WarehouseZone.A, size, isHeated, new DateTime(2022, 1, 2), new DateTime(2022, 1, 10));
        DateTime startDate = new DateTime(2024, 01,01);
        DateTime endDate = new DateTime(2024, 01, 02);
        double price = _priceService.CalculatePrice(warehouse.Size, warehouse.IsHeated, startDate, endDate);
        Assert.AreEqual(expectedPrice, price);
    }
    
    [TestMethod]
    public void CalculatePrice_InvalidWarehouseSize_ShouldThrowArgumentException()
    {
        Warehouse? warehouse = new Warehouse("Test Warehouse", WarehouseZone.A, (WarehouseSize) 4, false, new DateTime(2022, 1, 2), new DateTime(2022, 1, 10));
        DateTime startDate = new DateTime(2024, 01,01);
        DateTime endDate = new DateTime(2024, 01, 02);
        Assert.ThrowsException<ArgumentException>(() => _priceService.CalculatePrice(warehouse.Size, warehouse.IsHeated, startDate, endDate));
    }
    
    [TestMethod]
    public void CalculatePrice_DiscountMultiplierLessThan7Days_ShouldReturnExpectedPrice()
    {
        Warehouse? warehouse = new Warehouse("Test Warehouse", WarehouseZone.A, WarehouseSize.Small, false, new DateTime(2022, 1, 2), new DateTime(2022, 1, 10));
        DateTime startDate = new DateTime(2024, 01,01);
        DateTime endDate = new DateTime(2024, 01, 02);
        double price = _priceService.CalculatePrice(warehouse.Size, warehouse.IsHeated, startDate, endDate);
        Assert.AreEqual(50, price);
    }
    
    [TestMethod]
    public void CalculatePrice_DiscountMultiplier7To14Days_ShouldReturnExpectedPrice()
    {
        Warehouse? warehouse = new Warehouse("Test Warehouse", WarehouseZone.A, WarehouseSize.Small, false, new DateTime(2022, 1, 2), new DateTime(2022, 1, 10));
        DateTime startDate = new DateTime(2024, 01,01);
        DateTime endDate = new DateTime(2024, 01, 08);
        double price = _priceService.CalculatePrice(warehouse.Size, warehouse.IsHeated, startDate, endDate);
        Assert.AreEqual(332.5, price);
    }
    
    [TestMethod]
    public void CalculatePrice_DiscountMultiplierMoreThan14Days_ShouldReturnExpectedPrice()
    {
        Warehouse? warehouse = new Warehouse("Test Warehouse", WarehouseZone.A, WarehouseSize.Small, false, new DateTime(2022, 1, 2), new DateTime(2022, 1, 10));
        DateTime startDate = new DateTime(2024, 01,01);
        DateTime endDate = new DateTime(2024, 01, 16);
        double price = _priceService.CalculatePrice(warehouse.Size, warehouse.IsHeated, startDate, endDate);
        Assert.AreEqual(675, price);
    }
    
    [TestMethod]
    public void CalculatePrice_TotalDiscountForAvailablePromotions_ShouldReturnExpectedPrice()
    {
        _promotionRepo.Add(new Promotion("Some Promotion", 50, new DateTime(2024, 01, 01), new DateTime(2024, 01, 08)));
        Warehouse? warehouse = new Warehouse("Test Warehouse", WarehouseZone.A, WarehouseSize.Small, false, new DateTime(2022, 1, 2), new DateTime(2022, 1, 10));
        DateTime startDate = new DateTime(2024, 01,01);
        DateTime endDate = new DateTime(2024, 01, 02);
        double price = _priceService.CalculatePrice(warehouse.Size, warehouse.IsHeated, startDate, endDate);
        Assert.AreEqual(25, price);
    } 
    
}
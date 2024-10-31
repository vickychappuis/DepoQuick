using DepoQuick.Models;

namespace DepoQuick.Tests.Models;

[TestClass]
public class WarehouseModel
{
    private string _name = "Test Warehouse";
    private WarehouseZone _zone = WarehouseZone.A;
    private WarehouseSize _size = WarehouseSize.Large;
    private bool _isHeated = true;
    private DateTime _availableFrom = new DateTime(2022, 1, 1);
    private DateTime _availableTo = new DateTime(2022, 1, 10);
    
    [TestMethod]
    public void Warehouse_ShouldConstruct()
    {
        Warehouse warehouse = new Warehouse(_name, _zone, _size, _isHeated, _availableFrom, _availableTo);
        
        Assert.IsNotNull(warehouse);
        Assert.AreEqual(_zone, warehouse.Zone);
        Assert.AreEqual(_size, warehouse.Size);
        Assert.AreEqual(_isHeated, warehouse.IsHeated);
    }
    
    [TestMethod]
    public void WarehouseEquals_WithSameAttributes_ShouldReturnTrue()
    {
        Warehouse warehouse1 = new Warehouse(_name, _zone, _size, _isHeated, _availableFrom, _availableTo);
        Warehouse warehouse2 = new Warehouse(_name, _zone, _size, _isHeated, _availableFrom, _availableTo);
        
        Assert.IsTrue(warehouse1.Equals(warehouse2));
    }
    
    [TestMethod]
    public void WarehouseEquals_WithDifferentAttributes_ShouldReturnFalse()
    {
        Warehouse warehouse1 = new Warehouse(_name, _zone, _size, _isHeated, _availableFrom, _availableTo);
        Warehouse warehouse2 = new Warehouse("Another Warehouse", WarehouseZone.B, WarehouseSize.Medium, false, new DateTime(2022, 1, 1), new DateTime(2022, 1, 10));
        
        Assert.IsFalse(warehouse1.Equals(warehouse2));
    }
    
    [TestMethod]
    public void WarehouseEquals_WithNull_ShouldReturnFalse()
    {
        Warehouse warehouse1 = new Warehouse(_name, _zone, _size, _isHeated, _availableFrom, _availableTo);
        Warehouse? warehouse2 = null;
        
        Assert.IsFalse(warehouse1.Equals(warehouse2));
    }
    
    [TestMethod]
    public void WarehouseToString_ShouldReturnString()
    {
        Warehouse warehouse = new Warehouse("Testing", WarehouseZone.B, WarehouseSize.Large, true, _availableFrom, _availableTo) { WarehouseId = 18};
        
        string expected = "Testing (18) Large - B - Heated";
        Assert.AreEqual(expected, warehouse.ToString());
    }
}
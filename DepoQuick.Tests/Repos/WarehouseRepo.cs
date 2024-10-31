using DepoQuick.DataAccess;
using DepoQuick.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DepoQuick.Tests.Repos;

[TestClass]
public class WarehouseRepo
{
    private IDbContextFactory<Context> _contextFactory;
    private DataAccess.Repos.WarehouseRepo _warehouseRepo;
    
    [TestInitialize]
    public void TestInit()
    {
        var testsContext = new TestProgram();
        using var scope = testsContext.ServiceProvider.CreateScope();
        _contextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<Context>>();
        _warehouseRepo = new DataAccess.Repos.WarehouseRepo(_contextFactory);
    }

    [TestMethod]
    public void Add_ShouldAddWarehouse()
    {
        Warehouse warehouse = _warehouseRepo.Add(new Warehouse("A Warehouse", WarehouseZone.A, WarehouseSize.Large, true,
            new DateTime(2022, 1, 1), new DateTime(2022, 1, 5)));

        Assert.AreEqual(1, warehouse.WarehouseId);
        Assert.AreEqual(warehouse, _warehouseRepo.GetAll()[0]);
    }
    
    [TestMethod]
    public void Delete_ShouldDeleteWarehouse()
    {
        Warehouse warehouse = _warehouseRepo.Add(new Warehouse("A Warehouse", WarehouseZone.A, WarehouseSize.Large, true,
            new DateTime(2022, 1, 1), new DateTime(2022, 1, 5)));

        _warehouseRepo.Delete(warehouse.WarehouseId);

        Assert.AreEqual(0, _warehouseRepo.GetAll().Count);
    }
    
    [TestMethod]
    public void GetAll_ShouldReturnAllWarehouses()
    {
        Warehouse warehouse1 = _warehouseRepo.Add(new Warehouse("A Warehouse", WarehouseZone.A, WarehouseSize.Large, true,
            new DateTime(2022, 1, 1), new DateTime(2022, 1, 5)));
        Warehouse warehouse2 = _warehouseRepo.Add(new Warehouse("B Warehouse", WarehouseZone.B, WarehouseSize.Medium, false,
            new DateTime(2022, 1, 1), new DateTime(2022, 1, 5)));

        var warehouses = _warehouseRepo.GetAll();

        Assert.AreEqual(2, warehouses.Count);
        Assert.AreEqual(warehouse1, warehouses[0]);
        Assert.AreEqual(warehouse2, warehouses[1]);
    }
    
    [TestMethod]
    public void Get_FoundWarehouse_ShouldReturnWarehouse()
    {
        Warehouse warehouse = _warehouseRepo.Add(new Warehouse("A Warehouse", WarehouseZone.A, WarehouseSize.Large, true,
            new DateTime(2022, 1, 1), new DateTime(2022, 1, 5)));

        Warehouse foundWarehouse = _warehouseRepo.Get(warehouse.WarehouseId);

        Assert.AreEqual(warehouse, foundWarehouse);
    }
    
    
    [TestMethod]
    public void Get_NotFoundWarehouse_ShouldReturnNull()
    {
        Warehouse warehouse = _warehouseRepo.Add(new Warehouse("A Warehouse", WarehouseZone.A, WarehouseSize.Large, true,
            new DateTime(2022, 1, 1), new DateTime(2022, 1, 5)));

        Warehouse foundWarehouse = _warehouseRepo.Get(warehouse.WarehouseId + 1);

        Assert.IsNull(foundWarehouse);
    }
    
    [TestMethod]
    public void Update_ShouldUpdateWarehouse()
    {
        Warehouse warehouse = _warehouseRepo.Add(new Warehouse("A Warehouse", WarehouseZone.A, WarehouseSize.Large, true,
            new DateTime(2022, 1, 1), new DateTime(2022, 1, 5)));
        
        warehouse.Name = "B Warehouse";
        warehouse.Zone = WarehouseZone.B;
        warehouse.Size = WarehouseSize.Medium;
        warehouse.IsHeated = false;
        
        Warehouse updatedWarehouse = _warehouseRepo.Update(warehouse);

        Assert.AreEqual("B Warehouse", updatedWarehouse.Name);
        Assert.AreEqual(WarehouseZone.B, updatedWarehouse.Zone);
        Assert.AreEqual(WarehouseSize.Medium, updatedWarehouse.Size);
        Assert.IsFalse(updatedWarehouse.IsHeated);
    }
}
using DepoQuick.Backend.Dtos.Warehouses;
using DepoQuick.Backend.Models;
using DepoQuick.Backend.Providers;
using DepoQuick.Backend.Repos;
using DepoQuick.Backend.Services;

namespace DepoQuick.Tests.Services;

[TestClass]
public class WarehouseService
{
    private Backend.Services.WarehouseService _warehouseService;
    private ReservationRepo _reservationRepo;
    private WarehouseRepo _warehouseRepo;
    private InMemoryDatabase _db;
    
    [TestInitialize]
    public void TestInit()
    {
        _db = new InMemoryDatabase();
        _reservationRepo = new ReservationRepo(_db);
        _warehouseRepo = new WarehouseRepo(_db);
        _warehouseService = new Backend.Services.WarehouseService(_warehouseRepo, _reservationRepo);
        
        // Reset nextWarehouseId
        Warehouse.nextWarehouseId = 0;
    }
    
    [TestMethod]
    public void AddWarehouse_ShouldReturnWarehouse()
    {
        Warehouse warehouse = _warehouseService.AddWarehouse(WarehouseZone.A, WarehouseSize.Large, true);
        
        Assert.IsNotNull(warehouse);
        Assert.AreEqual(WarehouseZone.A, warehouse.Zone);
        Assert.AreEqual(WarehouseSize.Large, warehouse.Size);
        Assert.AreEqual(0,warehouse.Id);
        Assert.IsTrue(warehouse.IsHeated);
    }
    
    [TestMethod]
    [DataRow(WarehouseZone.A, WarehouseSize.Large, true, WarehouseZone.B, WarehouseSize.Medium, false)]
    [DataRow(WarehouseZone.B, WarehouseSize.Medium, true, WarehouseZone.B, WarehouseSize.Medium, true)]
    public void AddWarehouse_ShouldReturnUniqueIds(WarehouseZone zone1, WarehouseSize size1, bool isHeated1, WarehouseZone zone2, WarehouseSize size2, bool isHeated2)
    {
        Warehouse warehouse1 = _warehouseService.AddWarehouse(zone1, size1, isHeated1);
        Warehouse warehouse2 = _warehouseService.AddWarehouse(zone2, size2, isHeated2);

        Assert.AreNotEqual(warehouse1.Id, warehouse2.Id);
    }
    
    [TestMethod]
    [DataRow(WarehouseZone.A, WarehouseSize.Large, true, WarehouseZone.A, WarehouseSize.Large, true, WarehouseZone.A, WarehouseSize.Large, true)]
    [DataRow(WarehouseZone.A, WarehouseSize.Large, true, WarehouseZone.B, WarehouseSize.Large, true, WarehouseZone.C, WarehouseSize.Large, true)]

    public void AddWarehouse_ShouldAssignIdsIncrementally(WarehouseZone zone1, WarehouseSize size1, bool isHeated1, WarehouseZone zone2, WarehouseSize size2, bool isHeated2, WarehouseZone zone3, WarehouseSize size3, bool isHeated3)
    {
        Warehouse warehouse1 = _warehouseService.AddWarehouse(zone1, size1, isHeated1);
        Warehouse warehouse2 = _warehouseService.AddWarehouse(zone2, size2, isHeated2);
        Warehouse warehouse3 = _warehouseService.AddWarehouse(zone3, size3, isHeated3);
        
        Assert.AreEqual(warehouse1.Id + 1, warehouse2.Id);
        Assert.AreEqual(warehouse2.Id + 1, warehouse3.Id);
        Assert.AreEqual(warehouse1.Id + 2, warehouse3.Id);
    }
    
    [TestMethod]
    [DataRow(WarehouseZone.A, WarehouseSize.Large, true, 0)]
    public void AddWarehouse_ShouldAddWarehouseToDatabase(WarehouseZone zone, WarehouseSize size, bool isHeated, int expectedId)
    {
        Assert.AreEqual(0, _db.Warehouses.Count);
        
        Warehouse warehouse = _warehouseService.AddWarehouse(zone, size, isHeated);
        
        Assert.AreEqual(1, _db.Warehouses.Count);
        Assert.AreSame(warehouse, _db.Warehouses[0]);
    }
    
    [TestMethod]
    public void DeleteWarehouse_WarehouseFound_ShouldRemove()
    {
        Warehouse warehouse = _warehouseService.AddWarehouse(WarehouseZone.A, WarehouseSize.Large, true);
        
        Assert.AreEqual(1, _db.Warehouses.Count);
        
        _warehouseService.DeleteWarehouse(warehouse.Id);
        
        Assert.AreEqual(0, _db.Warehouses.Count);
    }
    
    [TestMethod]
    public void DeleteWarehouse_ShouldNotRemoveOtherWarehouses()
    {
        Warehouse warehouse1 = _warehouseService.AddWarehouse(WarehouseZone.A, WarehouseSize.Large, true);
        Warehouse warehouse2 = _warehouseService.AddWarehouse(WarehouseZone.B, WarehouseSize.Medium, false);
        
        Assert.AreEqual(2, _db.Warehouses.Count);
        
        _warehouseService.DeleteWarehouse(warehouse1.Id);
        
        Assert.AreEqual(1, _db.Warehouses.Count);
        Assert.AreSame(warehouse2, _db.Warehouses[0]);
    }

    [TestMethod]
    public void DeleteWarehouse_WarehouseNotFound_ShouldNotRemove()
    {
        Warehouse warehouse = _warehouseService.AddWarehouse(WarehouseZone.A, WarehouseSize.Large, true);

        Assert.AreEqual(1, _db.Warehouses.Count);

        _warehouseService.DeleteWarehouse(warehouse.Id + 1);

        Assert.AreEqual(1, _db.Warehouses.Count);
    }
    
    [TestMethod]
    [DataRow(55,ReservationStatus.Approved,null,4)]
    [DataRow(55,ReservationStatus.Pending,null,4)]
    public void DeleteWarehouse_WarehouseHasReservations_ShouldThrow(int reservationPrice, ReservationStatus reservationStatus, string rejectionNote, int clientId)
    {
        User client = new User("John Doe", "john@doe.com", "password", true);
        Warehouse warehouse = _warehouseService.AddWarehouse(WarehouseZone.A, WarehouseSize.Large, true);
        _reservationRepo.Add(new Reservation(DateTime.Now, DateTime.Now, warehouse, reservationPrice, reservationStatus, rejectionNote, client));
        
        Assert.ThrowsException<InvalidOperationException>(() => _warehouseService.DeleteWarehouse(warehouse.Id));
    }
    
    [TestMethod]
    public void AddWarehouse_GivenValidDto_ShouldCallAddWarehouse()
    {
        var dto = new AddWarehouseDto
        {
            Zone = WarehouseZone.A,
            Size = WarehouseSize.Large,
            IsHeated = true
        };
            
        var warehouse = _warehouseService.AddWarehouse(dto);
            
        Assert.AreEqual(1, _warehouseRepo.GetAll().Count);
        Assert.AreEqual(warehouse, _warehouseRepo.GetAll()[0]);
        
        Assert.AreEqual(WarehouseZone.A, dto.Zone);
        Assert.AreEqual(WarehouseSize.Large, dto.Size);
        Assert.IsTrue(dto.IsHeated);
    }
    
    [TestMethod]
    public void GetAll_ShouldReturnAllWarehouses()
    {
        var warehouse = _warehouseService.AddWarehouse(WarehouseZone.A, WarehouseSize.Large, true);
            
        var allWarehouses = _warehouseService.GetAllWarehouses();
            
        Assert.AreEqual(1, allWarehouses.Count);
        Assert.AreEqual(warehouse, allWarehouses[0]);
    }
}
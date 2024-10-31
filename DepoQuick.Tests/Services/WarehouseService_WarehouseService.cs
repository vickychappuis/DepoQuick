using DepoQuick.Backend.Dtos.Warehouses;
using DepoQuick.Backend.Services;
using DepoQuick.DataAccess;
using DepoQuick.Models;
using DepoQuick.DataAccess.Repos;
using Microsoft.Extensions.DependencyInjection;

namespace DepoQuick.Tests.Services;

[TestClass]
public class WarehouseService_WarehouseService
{
    private WarehouseService _warehouseService;
    private IRepo<Reservation, int> _reservationRepo;
    private IRepo<Warehouse, int> _warehouseRepo;

    [TestInitialize]
    public void TestInit()
    {
        var testsContext = new TestProgram();
        using var scope = testsContext.ServiceProvider.CreateScope();
        _reservationRepo = scope.ServiceProvider.GetRequiredService<IRepo<Reservation, int>>();
        _warehouseRepo = scope.ServiceProvider.GetRequiredService<IRepo<Warehouse, int>>();
        _warehouseService = new WarehouseService(_warehouseRepo, _reservationRepo);
    }


    [TestMethod]
    public void AddWarehouse_ShouldReturnWarehouse()
    {
        Warehouse warehouse = _warehouseService.AddWarehouse("A Warehouse", WarehouseZone.A, WarehouseSize.Large, true, new DateTime(2022, 1, 1), new DateTime(2022, 1, 2));

        Assert.IsNotNull(warehouse);
        Assert.AreEqual(WarehouseZone.A, warehouse.Zone);
        Assert.AreEqual(WarehouseSize.Large, warehouse.Size);
        Assert.AreEqual(1, warehouse.WarehouseId);
        Assert.IsTrue(warehouse.IsHeated);
    }

    [TestMethod]
    [DataRow(WarehouseZone.A, WarehouseSize.Large, true, WarehouseZone.B, WarehouseSize.Medium, false)]
    [DataRow(WarehouseZone.B, WarehouseSize.Medium, true, WarehouseZone.B, WarehouseSize.Medium, true)]
    public void AddWarehouse_ShouldReturnUniqueWarehouseIds(WarehouseZone zone1, WarehouseSize size1, bool isHeated1,
        WarehouseZone zone2, WarehouseSize size2, bool isHeated2)
    {
        Warehouse warehouse1 = _warehouseService.AddWarehouse("A Warehouse", zone1, size1, isHeated1, new DateTime(2022, 1, 1), new DateTime(2022, 1, 2));
        Warehouse warehouse2 = _warehouseService.AddWarehouse("Another Warehouse", zone2, size2, isHeated2, new DateTime(2022, 1, 1), new DateTime(2022, 1, 2));

        Assert.AreNotEqual(warehouse1.WarehouseId, warehouse2.WarehouseId);
    }

    [TestMethod]
    [DataRow(WarehouseZone.A, WarehouseSize.Large, true, WarehouseZone.A, WarehouseSize.Large, true, WarehouseZone.A,
        WarehouseSize.Large, true)]
    [DataRow(WarehouseZone.A, WarehouseSize.Large, true, WarehouseZone.B, WarehouseSize.Large, true, WarehouseZone.C,
        WarehouseSize.Large, true)]
    public void AddWarehouse_ShouldAssignWarehouseIdsIncrementally(WarehouseZone zone1, WarehouseSize size1,
        bool isHeated1, WarehouseZone zone2, WarehouseSize size2, bool isHeated2, WarehouseZone zone3,
        WarehouseSize size3, bool isHeated3)
    {
        Warehouse warehouse1 = _warehouseService.AddWarehouse("A Warehouse", zone1, size1, isHeated1, new DateTime(2022, 1, 1), new DateTime(2022, 1, 2));
        Warehouse warehouse2 = _warehouseService.AddWarehouse("Another Warehouse", zone2, size2, isHeated2, new DateTime(2022, 1, 1), new DateTime(2022, 1, 2));
        Warehouse warehouse3 = _warehouseService.AddWarehouse("Some Other Warehouse", zone3, size3, isHeated3, new DateTime(2022, 1, 1), new DateTime(2022, 1, 2));

        Assert.AreEqual(1, warehouse1.WarehouseId);
        Assert.AreEqual(2, warehouse2.WarehouseId);
        Assert.AreEqual(3, warehouse3.WarehouseId);
    }

    [TestMethod]
    [DataRow(WarehouseZone.A, WarehouseSize.Large, true, 0)]
    public void AddWarehouse_ShouldAddWarehouseToDatabase(WarehouseZone zone, WarehouseSize size, bool isHeated,
        int expectedId)
    {
        Assert.AreEqual(0, _warehouseRepo.GetAll().Count);

        Warehouse warehouse = _warehouseService.AddWarehouse("A Warehouse", zone, size, isHeated, new DateTime(2022, 1, 1), new DateTime(2022, 1, 2));

        Assert.AreEqual(1, _warehouseRepo.GetAll().Count);
        Assert.AreEqual(warehouse, _warehouseRepo.GetAll()[0]);
    }
    
    [TestMethod]
    public void AddWarehouse_AvailableToAfterAvailableFrom_ShouldThrow()
    {
        var beforeDate = new DateTime(2022, 01, 01);
        var afterDate = beforeDate.AddDays(2);

        Assert.ThrowsException<ArgumentException>(() =>
            _warehouseService.AddWarehouse("A Warehouse", WarehouseZone.A, WarehouseSize.Large, true, afterDate,
                beforeDate));
    }

    [TestMethod]
    public void DeleteWarehouse_WarehouseFound_ShouldRemove()
    {
        Warehouse warehouse = _warehouseService.AddWarehouse("A Warehouse", WarehouseZone.A, WarehouseSize.Large, true, new DateTime(2022, 1, 1), new DateTime(2022, 1, 2));

        Assert.AreEqual(1, _warehouseRepo.GetAll().Count);

        _warehouseService.DeleteWarehouse(warehouse.WarehouseId);

        Assert.AreEqual(0, _warehouseRepo.GetAll().Count);
    }

    [TestMethod]
    public void DeleteWarehouse_ShouldNotRemoveOtherWarehouses()
    {
        Warehouse warehouse1 = _warehouseService.AddWarehouse("A Warehouse", WarehouseZone.A, WarehouseSize.Large, true, new DateTime(2022, 1, 1), new DateTime(2022, 1, 2));
        Warehouse warehouse2 = _warehouseService.AddWarehouse("Another Warehouse", WarehouseZone.B, WarehouseSize.Medium, false, new DateTime(2022, 1, 1), new DateTime(2022, 1, 2));

        Assert.AreEqual(2, _warehouseRepo.GetAll().Count);

        _warehouseService.DeleteWarehouse(warehouse1.WarehouseId);

        Assert.AreEqual(1, _warehouseRepo.GetAll().Count);
        Assert.AreEqual(warehouse2, _warehouseRepo.GetAll()[0]);
    }

    [TestMethod]
    public void DeleteWarehouse_WarehouseNotFound_ShouldNotRemove()
    {
        Warehouse warehouse = _warehouseService.AddWarehouse("A Warehouse", WarehouseZone.A, WarehouseSize.Large, true, new DateTime(2022, 1, 1), new DateTime(2022, 1, 2));

        Assert.AreEqual(1, _warehouseRepo.GetAll().Count);

        _warehouseService.DeleteWarehouse(warehouse.WarehouseId + 1);

        Assert.AreEqual(1, _warehouseRepo.GetAll().Count);
    }

    [TestMethod]
    [DataRow(55, ReservationStatus.Approved, null, 4)]
    [DataRow(55, ReservationStatus.Pending, null, 4)]
    public void DeleteWarehouse_WarehouseHasReservations_ShouldThrow(int reservationPrice,
        ReservationStatus reservationStatus, string rejectionNote, int clientId)
    {
        User client = new User("John Doe", "john@doe.com", "password", true);
        Warehouse warehouse = _warehouseService.AddWarehouse("A Warehouse", WarehouseZone.A, WarehouseSize.Large, true, new DateTime(2022, 1, 1), new DateTime(2022, 1, 2));
        _reservationRepo.Add(
            new Reservation
            {
                StartDate = DateTime.Now,
                EndDate = DateTime.Now,
                WarehouseId = warehouse.WarehouseId,
                Price = reservationPrice,
                Status = reservationStatus,
                RejectionNote = rejectionNote,
                Client = client
            });

        Assert.ThrowsException<InvalidOperationException>(() => _warehouseService.DeleteWarehouse(warehouse.WarehouseId));
    }

    [TestMethod]
    public void AddWarehouse_GivenValidDto_ShouldCallAddWarehouse()
    {
        var dto = new AddWarehouseDto
        {
            Name = "Test Warehouse",
            Zone = WarehouseZone.A,
            Size = WarehouseSize.Large,
            IsHeated = true,
            AvailableFrom = new DateTime(2022, 1, 1),
            AvailableTo = new DateTime(2022, 1, 2)
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
        var warehouse = _warehouseService.AddWarehouse("A Warehouse", WarehouseZone.A, WarehouseSize.Large, true, new DateTime(2022, 1, 1), new DateTime(2022, 1, 2));

        var allWarehouses = _warehouseService.GetAllWarehouses();

        Assert.AreEqual(1, allWarehouses.Count);
        Assert.AreEqual(warehouse, allWarehouses[0]);
    }
}
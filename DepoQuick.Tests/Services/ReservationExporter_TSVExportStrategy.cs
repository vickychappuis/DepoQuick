using System.Text;
using DepoQuick.Models;
using DepoQuick.Backend.Services;
using DepoQuick.DataAccess;
using DepoQuick.DataAccess.Repos;
using Microsoft.Extensions.DependencyInjection;

namespace DepoQuick.Tests.Services;

[TestClass]
public class ReservationExporter_TSVExportStrategy
{
    private IRepo<Warehouse, int> _warehouseRepo;
    private IRepo<Reservation, int> _reservationRepo;
    private IRepo<User, int> _userRepo;

    [TestInitialize]
    public void TestInit()
    {
        var testsContext = new TestProgram();
        using var scope = testsContext.ServiceProvider.CreateScope();
        _warehouseRepo = scope.ServiceProvider.GetRequiredService<IRepo<Warehouse, int>>();
        _reservationRepo = scope.ServiceProvider.GetRequiredService<IRepo<Reservation, int>>();
        _userRepo = scope.ServiceProvider.GetRequiredService<IRepo<User, int>>();
    }
    
    [TestMethod]
    public void ExportReservations_WithTSVExportStrategy_ShouldSetHeader()
    {
        var reservations = _reservationRepo.GetAll();
        var exportStrategy = new TSVExportStrategy();
        var reservationExporter = new ReservationExporter(exportStrategy);
        var result = reservationExporter.ExportReservations(reservations);
        
        Assert.IsTrue(result.StartsWith("data:text/tab-separated-values;base64,"));
    }

    [TestMethod]
    public void ExportReservations_WithTSVExportStrategy_ShouldSetContent()
    {
        var client = _userRepo.Add(new User("john", "john@doe.com", "testPass!1", true));
        
        var warehouse = _warehouseRepo.Add(new Warehouse("A Warehouse", WarehouseZone.A, WarehouseSize.Large, true,
            new DateTime(2022, 1, 1), new DateTime(2022, 1, 5)));
        
        var reservation = _reservationRepo.Add(new Reservation
        {
            StartDate = new DateTime(2022, 1, 2),
            EndDate = new DateTime(2022, 1, 4),
            WarehouseId = warehouse.WarehouseId,
            Price = 222,
            Status = ReservationStatus.Pending,
            ClientId = client.UserId
        });

        var reservations = _reservationRepo.GetAll();
        var exportStrategy = new TSVExportStrategy();
        var reservationExporter = new ReservationExporter(exportStrategy);
        var result = reservationExporter.ExportReservations(reservations);
        
        var expected = $"data:text/tab-separated-values;base64,{Convert.ToBase64String(Encoding.UTF8.GetBytes($"DEPOSITO\tRESERVA\tPAGO\n{warehouse.WarehouseId}\t{reservation.ReservationId}\tnulo\n"))}";

        Assert.AreEqual(expected, result);
    }
}
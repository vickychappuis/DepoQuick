using DepoQuick.DataAccess;
using DepoQuick.DataAccess.Repos;
using DepoQuick.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DepoQuick.Tests.Repos;

[TestClass]
public class ReservationRepo
{
    private IDbContextFactory<Context> _contextFactory;
    private IRepo<Warehouse, int> _warehouseRepo;
    private IRepo<User, int> _userRepo;
    private DataAccess.Repos.ReservationRepo _reservationRepo;

    [TestInitialize]
    public void TestInit()
    {
        var testsContext = new TestProgram();
        using var scope = testsContext.ServiceProvider.CreateScope();
        _contextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<Context>>();
        _warehouseRepo = scope.ServiceProvider.GetRequiredService<IRepo<Warehouse, int>>();
        _userRepo = scope.ServiceProvider.GetRequiredService<IRepo<User, int>>();
        _reservationRepo = new DataAccess.Repos.ReservationRepo(_contextFactory);
    }

    [TestMethod]
    public void Add_ShouldAddReservation()
    {
        _warehouseRepo.Add(new Warehouse("A Warehouse", WarehouseZone.A, WarehouseSize.Large, true,
            new DateTime(2022, 1, 1), new DateTime(2022, 1, 5)));

        _userRepo.Add(new User("john", "john@doe.com", "testPass!1", true));

        Reservation reservation =
            _reservationRepo.Add(new Reservation
            {
                StartDate = new DateTime(2022, 1, 1),
                EndDate = new DateTime(2022, 1, 5),
                WarehouseId = 1,
                Price = 100,
                Status = ReservationStatus.Pending,
                RejectionNote = null,
                PaymentStatus = null,
                ClientId = 1
            });

        Assert.AreEqual(1, reservation.ReservationId);
        Assert.AreEqual(reservation, _reservationRepo.GetAll()[0]);
    }

    [TestMethod]
    public void Delete_ShouldDeleteReservation()
    {
        _warehouseRepo.Add(new Warehouse("A Warehouse", WarehouseZone.A, WarehouseSize.Large, true,
            new DateTime(2022, 1, 1), new DateTime(2022, 1, 5)));

        _userRepo.Add(new User("john", "john@doe.com", "testPass!1", true));

        Reservation reservation =
            _reservationRepo.Add(new Reservation
            {
                StartDate = new DateTime(2022, 1, 1),
                EndDate = new DateTime(2022, 1, 5),
                WarehouseId = 1,
                Price = 100,
                Status = ReservationStatus.Pending,
                RejectionNote = null,
                PaymentStatus = null,
                ClientId = 1
            });

        _reservationRepo.Delete(reservation.ReservationId);

        Assert.AreEqual(0, _reservationRepo.GetAll().Count);
    }

    [TestMethod]
    public void GetAll_ShouldReturnAllReservations()
    {
        _warehouseRepo.Add(new Warehouse("A Warehouse", WarehouseZone.A, WarehouseSize.Large, true,
            new DateTime(2022, 1, 1), new DateTime(2022, 1, 5)));

        _userRepo.Add(new User("john", "john@doe.com", "testPass!1", true));

        Reservation reservation1 =
            _reservationRepo.Add(new Reservation
            {
                StartDate = new DateTime(2022, 1, 1),
                EndDate = new DateTime(2022, 1, 5),
                WarehouseId = 1,
                Price = 100,
                Status = ReservationStatus.Pending,
                RejectionNote = null,
                PaymentStatus = null,
                ClientId = 1
            });
        Reservation reservation2 =
            _reservationRepo.Add(new Reservation
            {
                StartDate = new DateTime(2022, 1, 1),
                EndDate = new DateTime(2022, 1, 5),
                WarehouseId = 1,
                Price = 100,
                Status = ReservationStatus.Pending,
                RejectionNote = null,
                PaymentStatus = null,
                ClientId = 1
            });

        var reservations = _reservationRepo.GetAll();

        Assert.AreEqual(2, reservations.Count);
        Assert.AreEqual(reservation1, reservations[0]);
        Assert.AreEqual(reservation2, reservations[1]);
    }

    [TestMethod]
    public void Get_FoundReservation_ShouldReturnReservation()
    {
        _warehouseRepo.Add(new Warehouse("A Warehouse", WarehouseZone.A, WarehouseSize.Large, true,
            new DateTime(2022, 1, 1), new DateTime(2022, 1, 5)));

        _userRepo.Add(new User("john", "john@doe.com", "testPass!1", true));

        Reservation reservation =
            _reservationRepo.Add(new Reservation
            {
                StartDate = new DateTime(2022, 1, 1),
                EndDate = new DateTime(2022, 1, 5),
                WarehouseId = 1,
                Price = 100,
                Status = ReservationStatus.Pending,
                RejectionNote = null,
                PaymentStatus = null,
                ClientId = 1
            });

        Assert.AreEqual(reservation, _reservationRepo.Get(reservation.ReservationId));
    }

    [TestMethod]
    public void Get_NotFoundReservation_ShouldReturnNull()
    {
        _warehouseRepo.Add(new Warehouse("A Warehouse", WarehouseZone.A, WarehouseSize.Large, true,
            new DateTime(2022, 1, 1), new DateTime(2022, 1, 5)));

        _userRepo.Add(new User("john", "john@doe.com", "testPass!1", true));

        Reservation reservation =
            _reservationRepo.Add(new Reservation
            {
                StartDate = new DateTime(2022, 1, 1),
                EndDate = new DateTime(2022, 1, 5),
                WarehouseId = 1,
                Price = 100,
                Status = ReservationStatus.Pending,
                RejectionNote = null,
                PaymentStatus = null,
                ClientId = 1
            });

        Assert.IsNull(_reservationRepo.Get(reservation.ReservationId + 1));
    }

    [TestMethod]
    public void Update_ShouldUpdateReservation()
    {
        _warehouseRepo.Add(new Warehouse("A Warehouse", WarehouseZone.A, WarehouseSize.Large, true,
            new DateTime(2022, 1, 1), new DateTime(2022, 1, 5)));

        _userRepo.Add(new User("john", "john@doe.com", "testPass!1", true));

        Reservation reservation =
            _reservationRepo.Add(new Reservation
            {
                StartDate = new DateTime(2022, 1, 1),
                EndDate = new DateTime(2022, 1, 5),
                WarehouseId = 1,
                Price = 100,
                Status = ReservationStatus.Pending,
                RejectionNote = null,
                PaymentStatus = null,
                ClientId = 1
            });

        reservation.Status = ReservationStatus.Rejected;
        reservation.RejectionNote = "No reason :(";

        _reservationRepo.Update(reservation);

        Assert.AreEqual(ReservationStatus.Rejected, _reservationRepo.Get(reservation.ReservationId).Status);
        Assert.AreEqual("No reason :(", _reservationRepo.Get(reservation.ReservationId).RejectionNote);
    }

    [TestMethod]
    public void Delete_NotFoundReservation_ShouldNotDeleteReservation()
    {
        _warehouseRepo.Add(new Warehouse("A Warehouse", WarehouseZone.A, WarehouseSize.Large, true,
            new DateTime(2022, 1, 1), new DateTime(2022, 1, 5)));

        _userRepo.Add(new User("john", "john@doe.com", "testPass!1", true));

        Reservation reservation =
            _reservationRepo.Add(new Reservation
            {
                StartDate = new DateTime(2022, 1, 1),
                EndDate = new DateTime(2022, 1, 5),
                WarehouseId = 1,
                Price = 100,
                Status = ReservationStatus.Pending,
                RejectionNote = null,
                PaymentStatus = null,
                ClientId = 1
            });

        _reservationRepo.Delete(reservation.ReservationId + 1);

        Assert.AreEqual(1, _reservationRepo.GetAll().Count);
    }
}
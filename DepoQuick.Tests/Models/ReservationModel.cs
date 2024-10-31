using DepoQuick.Models;

namespace DepoQuick.Tests.Models;

[TestClass]
public class ReservationModel
{
    private int _validReservationId = 1;
    private DateTime _validStartDate = new DateTime(2022, 1, 1);
    private DateTime _validEndDate = new DateTime(2022, 1, 10);
    private double _validPrice = 0;
    private ReservationStatus _validStatus = ReservationStatus.Pending;
    private string? _validRejectionNote = null;
    private Warehouse? _validWarehouse = new Warehouse("Test Warehouse", WarehouseZone.A, WarehouseSize.Large, true, new DateTime(2022, 1, 1), new DateTime(2022, 1, 10));
    private int _validClientId = 1;
    private User _validClient = new User("John Doe", "john@doe.com", "password", true);
    private Reservation _validReservation;

    [TestInitialize]
    public void Setup()
    {
        _validReservation = new Reservation
        {
            StartDate = _validStartDate,
            EndDate = _validEndDate,
            Warehouse = _validWarehouse,
            Price = _validPrice,
            Status = _validStatus,
            RejectionNote = _validRejectionNote,
            Client = _validClient
        };

    }
    
    
    [TestMethod]
    public void Reservation_ShouldConstruct()
    {
        Reservation reservation = new Reservation
        {
            ReservationId = _validReservationId,
            StartDate = _validStartDate,
            EndDate = _validEndDate,
            Warehouse = _validWarehouse,
            Price = _validPrice,
            Status = _validStatus,
            RejectionNote = _validRejectionNote,
            ClientId = _validClientId,
            Client = _validClient
        };
        
        Assert.IsNotNull(reservation);
        Assert.AreEqual(_validReservationId,reservation.ReservationId);
        Assert.AreEqual(_validStartDate, reservation.StartDate);
        Assert.AreEqual(_validEndDate, reservation.EndDate);
        Assert.AreEqual(_validWarehouse, reservation.Warehouse);
        Assert.AreEqual(_validPrice, reservation.Price);
        Assert.AreEqual(_validClient, reservation.Client);
        Assert.AreEqual(_validStatus, reservation.Status);
        Assert.AreEqual(_validRejectionNote, reservation.RejectionNote);
        Assert.IsNull(reservation.PaymentStatus);
        Assert.AreEqual(_validClientId, reservation.ClientId);
        Assert.AreEqual(_validClient, reservation.Client);
        
    }
    
    [TestMethod]
    public void Equals_WithIdenticalReservations_ShouldReturnTrue()
    {
        var reservation1 = _validReservation;
        var reservation2 = _validReservation;
        Assert.IsTrue(reservation1.Equals(reservation2));
    }
    
    [TestMethod]
    public void Equals_WithDifferentReservations_ShouldReturnFalse()
    {
        var reservation1 = _validReservation;
        double differentPrice = _validPrice + 1;
        var reservation2 = new Reservation
        {
            StartDate = _validStartDate,
            EndDate = _validEndDate,
            Warehouse = _validWarehouse,
            Price = differentPrice,
            Status = _validStatus,
            RejectionNote = _validRejectionNote,
            Client = _validClient
        };
        Assert.IsFalse(reservation1.Equals(reservation2));
    }
    
    [TestMethod]
    public void Equals_WithNull_ReturnsFalse()
    {
        var reservation1 = _validReservation;
        Reservation reservation2 = null;

        Assert.IsFalse(reservation1.Equals(reservation2));
    }
}
using DepoQuick.Backend.Dtos.Reservations;
using DepoQuick.Models;
using DepoQuick.DataAccess.Repos;
using DepoQuick.Backend.Services;
using DepoQuick.DataAccess;
using Microsoft.Extensions.DependencyInjection;

namespace DepoQuick.Tests.Services
{
    [TestClass]
    public class ReservationService_ReservationService
    {
        private IRepo<Reservation, int> _reservationRepo;
        private IRepo<Warehouse, int> _warehouseRepo;
        private IRepo<Promotion, int> _promotionRepo;
        private IRepo<User, int> _userRepo;
        private PromotionService _promotionService;
        private PriceService _priceService;
        private ReservationService _reservationService;
        
        private DateTime _validStartDate = DateTime.Parse("2025-01-01");
        private DateTime _validEndDate = DateTime.Parse("2025-01-08");
        private Warehouse _validWarehouse = new Warehouse("Test Warehouse", WarehouseZone.A, WarehouseSize.Small, true, new DateTime(2025, 1, 1), new DateTime(2025, 1, 8));
        private User _validClient = new User("John Doe", "john@doe.com", "password", true);
        
        [TestInitialize]
        public void TestInit()
        {
            var testsContext = new TestProgram();
            using var scope = testsContext.ServiceProvider.CreateScope();
            _reservationRepo = scope.ServiceProvider.GetRequiredService<IRepo<Reservation, int>>();
            _warehouseRepo = scope.ServiceProvider.GetRequiredService<IRepo<Warehouse, int>>();
            _promotionRepo = scope.ServiceProvider.GetRequiredService<IRepo<Promotion, int>>();
            _promotionService = new PromotionService(_promotionRepo);
            _userRepo = scope.ServiceProvider.GetRequiredService<IRepo<User, int>>();
            _priceService = new PriceService(_promotionRepo);
            _reservationService = new ReservationService(_reservationRepo, _warehouseRepo, _priceService);
        }

        [TestMethod]
        public void AddReservation_WithValidData_ShouldAddToDatabase()
        {
            var client = _userRepo.Add(_validClient);
            var warehouse = _warehouseRepo.Add(_validWarehouse);
            
            var reservation = _reservationService.AddReservation(_validStartDate, _validEndDate, warehouse.WarehouseId, client.UserId);
            
            Assert.AreEqual(1, _reservationRepo.GetAll().Count);
            Assert.AreEqual(reservation, _reservationRepo.GetAll()[0]);
        }
        
        [TestMethod]
        public void AddReservation_EndDateBeforeStartDate_ShouldThrow()
        {
            DateTimeService.CurrentDateTime = DateTime.Parse("2022-01-01");
            _userRepo.Add(_validClient);
            
            var beforeDate = DateTime.Parse("2022-01-07");
            var afterDate = DateTime.Parse("2022-01-09");

            Assert.ThrowsException<ArgumentException>(() =>
                _reservationService.AddReservation(afterDate, beforeDate, _validWarehouse.WarehouseId, _validClient.UserId));
        }
        
        [TestMethod]
        public void AddReservation_StartDateInPast_ShouldThrow()
        {
            DateTimeService.CurrentDateTime = DateTime.Parse("2023-01-01");
            _userRepo.Add(_validClient);
            
            var pastDate = DateTime.Parse("2021-01-01");
            var afterPastDate = DateTime.Parse("2021-01-03");
            
            Assert.ThrowsException<ArgumentException>(() =>
                _reservationService.AddReservation(pastDate, afterPastDate, _validWarehouse.WarehouseId, _validClient.UserId));
        }
        
        [TestMethod]
        [DataRow("2025-01-03", "2025-01-07", "2025-01-05", "2025-01-08")]
        [DataRow("2025-01-03", "2025-01-07", "2025-01-02", "2025-01-05")]
        [DataRow("2025-01-03", "2025-01-07", "2025-01-04", "2025-01-06")]
        [DataRow("2025-01-03", "2025-01-07", "2025-01-02", "2025-01-08")]
        [DataRow("2025-01-03", "2025-01-07", "2025-01-03", "2025-01-07")]
        public void AddReservation_WithOverlappingReservation_ShouldThrow(string start, string end, string start2, string end2)
        {
            
            var startDate1 = DateTime.Parse(start);
            var endDate1 = DateTime.Parse(end);
            
            var startDate2 = DateTime.Parse(start2);
            var endDate2 = DateTime.Parse(end2);
            
            
            var client = _userRepo.Add(_validClient);
            var client2 = _userRepo.Add(new User("Jane Doe", "jane@doe.com", "password", false));
            var warehouse = _warehouseRepo.Add(_validWarehouse);
           
            _reservationService.AddReservation(startDate1, endDate1, warehouse.WarehouseId, client2.UserId);
            
            Assert.ThrowsException<ArgumentException>(() =>
                _reservationService.AddReservation(startDate2, endDate2, warehouse.WarehouseId, client.UserId));
        }
        
        [TestMethod]
        public void AddReservation_OutsideWarehouseAvailability_ShouldThrow()
        {
            var client = _userRepo.Add(_validClient);
            var warehouse = _warehouseRepo.Add(_validWarehouse);
            
            Assert.ThrowsException<ArgumentException>(() =>
                _reservationService.AddReservation(_validWarehouse.AvailableFrom.AddDays(2), _validWarehouse.AvailableTo.AddDays(1), warehouse.WarehouseId, client.UserId));
        }
        
        [TestMethod]
        public void AddReservation_GivenValidDto_ShouldCallAddReservation()
        {
            _userRepo.Add(_validClient);
            _warehouseRepo.Add(_validWarehouse);
            
            var dto = new AddReservationDto
            {
                StartDate = _validStartDate,
                EndDate = _validEndDate,
                WarehouseId = _validWarehouse.WarehouseId
            };
            
            var reservation = _reservationService.AddReservation(dto, _validClient.UserId);
            
            Assert.AreEqual(1, _reservationRepo.GetAll().Count);
            Assert.AreEqual(reservation, _reservationRepo.GetAll()[0]);
        }
        
        [TestMethod]
        public void AddReservation_GivenDtoWithoutWarehouseId_ShouldThrow()
        {
            _userRepo.Add(_validClient);
            
            var dto = new AddReservationDto
            {
                StartDate = _validStartDate,
                EndDate = _validEndDate,
                WarehouseId = null
            };
            
            Assert.ThrowsException<ArgumentNullException>(() => _reservationService.AddReservation(dto, _validClient.UserId));
        }
        
        [TestMethod]
        public void AddReservation_GivenDtoWithNonExistentWarehouseId_ShouldThrow()
        {
           const int invalidWarehouseId = 1;
            
            var dto = new AddReservationDto
            {
                StartDate = _validStartDate,
                EndDate = _validEndDate,
                WarehouseId = invalidWarehouseId
            };
            Assert.ThrowsException<InvalidOperationException>(() => _reservationService.AddReservation(dto, _validClient.UserId));
        }

        [TestMethod]
        public void GetAllClientReservations_GivenClientId_ShouldReturnClientReservations()
        {
            _userRepo.Add(_validClient);
            _warehouseRepo.Add(_validWarehouse);
            
            var reservation = _reservationService.AddReservation(_validStartDate, _validEndDate, _validWarehouse.WarehouseId, _validClient.UserId);
            
            var clientReservations = _reservationService.GetAllClientReservations(_validClient.UserId);
            
            Assert.AreEqual(1, clientReservations.Count);
            Assert.AreEqual(reservation, clientReservations[0]);
        }
        
        [TestMethod]
        public void AddReservation_GivenDtoWithNonExistentClientId_ShouldThrow()
        {
            var warehouse = _warehouseRepo.Add(_validWarehouse);
            
            var dto = new AddReservationDto
            {
                StartDate = _validStartDate,
                EndDate = _validEndDate,
                WarehouseId = warehouse.WarehouseId
            };
            Assert.ThrowsException<InvalidOperationException>(() => _reservationService.AddReservation(dto, _validClient.UserId));
        }
        
        [TestMethod]
        public void UpdateReservation_GivenApprovedDtoWithRejectionNote_ShouldThrow()
        {
            var client = _userRepo.Add(_validClient);
            var warehouse = _warehouseRepo.Add(_validWarehouse);
            
            var reservation = _reservationService.AddReservation(_validStartDate, _validEndDate, warehouse.WarehouseId, client.UserId);
            var updateDto = new UpdateReservationDto
            {
                IsApproved = true,
                RejectionNote = "Note"
            };
            
            Assert.ThrowsException<Exception>(() => _reservationService.UpdateReservationStatus(reservation.ReservationId, updateDto));
        }
        
        [TestMethod]
        public void UpdateReservation_GivenValidDto_ShouldUpdateReservation()
        {
            _userRepo.Add(_validClient);
            _warehouseRepo.Add(_validWarehouse);
            
            var reservation = _reservationService.AddReservation(_validStartDate, _validEndDate, _validWarehouse.WarehouseId, _validClient.UserId);
            var updateDto = new UpdateReservationDto
            {
                IsApproved = true
            };
            
            var updatedReservation = _reservationService.UpdateReservationStatus(reservation.ReservationId, updateDto);
            
            Assert.AreEqual(ReservationStatus.Approved, updatedReservation.Status);
        }
        
        [TestMethod]
        public void UpdateReservation_GivenValidDtoWithRejectionNote_ShouldUpdateReservation()
        {
            _userRepo.Add(_validClient);
            _warehouseRepo.Add(_validWarehouse);
            
            var reservation = _reservationService.AddReservation(_validStartDate, _validEndDate, _validWarehouse.WarehouseId,  _validClient.UserId);
            var updateDto = new UpdateReservationDto
            {
                IsApproved = false,
                RejectionNote = "Note"
            };
            
            var updatedReservation = _reservationService.UpdateReservationStatus(reservation.ReservationId, updateDto);
            
            Assert.AreEqual(ReservationStatus.Rejected, updatedReservation.Status);
            Assert.AreEqual("Note", updatedReservation.RejectionNote);
        }
        
        [TestMethod]
        public void UpdateReservation_GivenValidDtoWithNullRejectionNote_ShouldUpdateReservation()
        {
            _userRepo.Add(_validClient);
            _warehouseRepo.Add(_validWarehouse);
            
            var reservation = _reservationService.AddReservation(_validStartDate, _validEndDate, _validWarehouse.WarehouseId,  _validClient.UserId);
            var updateDto = new UpdateReservationDto
            {
                IsApproved = false,
                RejectionNote = null
            };
            
            var updatedReservation = _reservationService.UpdateReservationStatus(reservation.ReservationId, updateDto);
            
            Assert.AreEqual(ReservationStatus.Rejected, updatedReservation.Status);
            Assert.IsNull(updatedReservation.RejectionNote);
        }

        [TestMethod]
        public void UpdateReservation_WithApprovedDtoGivenNonExistentId_ShouldThrow()
        {
            const int nonExistentReservationId = 1;
            
            var updateDto = new UpdateReservationDto
            {
                IsApproved = true,
            };
            
            Assert.ThrowsException<InvalidOperationException>(() => _reservationService.UpdateReservationStatus(nonExistentReservationId, updateDto));
        }
        
        [TestMethod]
        public void UpdateReservation_WithDisapprovedDtoGivenNonExistentId_ShouldThrow()
        {
            const int nonExistentReservationId = 1;
            
            var updateDto = new UpdateReservationDto
            {
                IsApproved = false,
            };
            
            Assert.ThrowsException<InvalidOperationException>(() => _reservationService.UpdateReservationStatus(nonExistentReservationId, updateDto));
        }
        
        [TestMethod]
        public void UpdateReservation_GivenDtoWithRejectionLongerThan300Chars_ShouldThrow()
        {
            var client = _userRepo.Add(_validClient);
            var warehouse = _warehouseRepo.Add(_validWarehouse);
            
            var reservation = _reservationService.AddReservation(_validStartDate, _validEndDate, warehouse.WarehouseId, client.UserId);
            var updateDto = new UpdateReservationDto
            {
                IsApproved = false,
                RejectionNote = new string('v', 301)
            };
            
            Assert.ThrowsException<ArgumentException>(() => _reservationService.UpdateReservationStatus(reservation.ReservationId, updateDto));
        }

        [DataRow(PaymentStatus.Captured)]
        [DataRow(PaymentStatus.Reserved)]
        [TestMethod]
        public void UpdateReservationPaymentStatus_GivenValidId_ShouldUpdatePaymentStatus(PaymentStatus paymentStatus)
        {
            var client = _userRepo.Add(_validClient);
            var warehouse = _warehouseRepo.Add(_validWarehouse);
            
            var reservation = _reservationService.AddReservation(_validStartDate, _validEndDate, warehouse.WarehouseId, client.UserId);
            
            var updatedReservation = _reservationService.UpdateReservationPaymentStatus(reservation.ReservationId, paymentStatus);
            
            Assert.AreEqual(paymentStatus, updatedReservation.PaymentStatus);
        }
        
        [TestMethod]
        public void UpdateReservationPaymentStatus_GivenNonExistentId_ShouldThrow()
        {
            const int nonExistentReservationId = 1;
            
            Assert.ThrowsException<InvalidOperationException>(() => _reservationService.UpdateReservationPaymentStatus(nonExistentReservationId, PaymentStatus.Captured));
        }
        
        [TestMethod]
        public void GetAll_ShouldReturnAllReservations()
        {
            var client =_userRepo.Add(_validClient);
            var warehouse = _warehouseRepo.Add(_validWarehouse);
            
            var reservation = _reservationService.AddReservation(_validStartDate, _validEndDate, warehouse.WarehouseId, client.UserId);
            
            var allReservations = _reservationService.GetAll();
            
            Assert.AreEqual(1, allReservations.Count);
            Assert.AreEqual(reservation, allReservations[0]);
        }
        
        [TestMethod]
        public void GetAll_GivenNoReservations_ShouldReturnEmptyList()
        {
            var allReservations = _reservationService.GetAll();
            
            Assert.AreEqual(0, allReservations.Count);
        }
        
        [TestMethod]
        public void AddReservation_GivenPastStartDate_ShouldThrow()
        {
            var pastDate = DateTime.Parse("2021-01-01");
            
            Assert.ThrowsException<ArgumentException>(() => _reservationService.AddReservation(pastDate, _validEndDate, _validWarehouse.WarehouseId, _validClient.UserId));
        }
        
        [TestMethod]
        public void AddReservation_GivenNonExistentClient_ShouldThrow()
        {
            const int nonExistentClientId = 1;
            
            Assert.ThrowsException<InvalidOperationException>(() => _reservationService.AddReservation(_validStartDate, _validEndDate, _validWarehouse.WarehouseId, nonExistentClientId));
        }
        
        [TestMethod]
        public void UpdateReservation_GivenApprovedDto_ShouldUpdatePaymentStatus()
        {
            var client = _userRepo.Add(_validClient);
            var warehouse = _warehouseRepo.Add(_validWarehouse);
            
            var reservation = _reservationService.AddReservation(_validStartDate, _validEndDate, warehouse.WarehouseId, client.UserId);
            var updateDto = new UpdateReservationDto
            {
                IsApproved = true
            };
            
            var updatedReservation = _reservationService.UpdateReservationStatus(reservation.ReservationId, updateDto);
            
            Assert.AreEqual(updatedReservation.PaymentStatus, PaymentStatus.Captured);
        }
        
        [TestMethod]
        public void UpdateReservation_GivenDisapprovedDto_ShouldUpdatePaymentStatus()
        {
            var client = _userRepo.Add(_validClient);
            var warehouse = _warehouseRepo.Add(_validWarehouse);
            
            var reservation = _reservationService.AddReservation(_validStartDate, _validEndDate, warehouse.WarehouseId, client.UserId);
            var updateDto = new UpdateReservationDto
            {
                IsApproved = false
            };
            
            var updatedReservation = _reservationService.UpdateReservationStatus(reservation.ReservationId, updateDto);
            
            Assert.IsNull(updatedReservation.PaymentStatus);
        }
        
        [TestMethod]
        public void AddReservation_WithNonExistentClient_ShouldThrowInvalidOperationException()
        {
            int nonExistentClientId = -99999;
            var warehouse = _warehouseRepo.Add(_validWarehouse);

            Assert.ThrowsException<InvalidOperationException>(() => _reservationService.AddReservation(_validStartDate, _validEndDate, warehouse.WarehouseId, nonExistentClientId));
        }
        
        [TestMethod]
        public void GetAvailableWarehouses_WithAvailableWarehouses_ShouldReturnAvailableWarehouses()
        {
            var warehouse = _warehouseRepo.Add(_validWarehouse);
    
            var availableWarehouses = _reservationService.GetAvailableWarehouses(_validStartDate, _validEndDate);
    
            Assert.AreEqual(1, availableWarehouses.Count);
            Assert.AreEqual(warehouse, availableWarehouses[0]);
        }

        [TestMethod]
        public void GetAvailableWarehouses_WithUnavailableWarehouses_ShouldReturnEmptyList()
        {
            _warehouseRepo.Add(_validWarehouse);
    
            var unavailableStartDate = _validWarehouse.AvailableFrom.AddDays(1);
            var unavailableEndDate = _validWarehouse.AvailableTo.AddDays(1);
    
            var availableWarehouses = _reservationService.GetAvailableWarehouses(unavailableStartDate, unavailableEndDate);
    
            Assert.AreEqual(0, availableWarehouses.Count);
        }

        [TestMethod]
        public void GetAvailableWarehouses_WithStartDateAfterEndDate_ShouldThrow()
        {
            Assert.ThrowsException<ArgumentException>(() => _reservationService.GetAvailableWarehouses(_validEndDate, _validStartDate));
        }

        [TestMethod]
        public void GetAvailableWarehouses_WithNoWarehouses_ShouldReturnEmptyList()
        {
            var availableWarehouses = _reservationService.GetAvailableWarehouses(_validStartDate, _validEndDate);
    
            Assert.AreEqual(0, availableWarehouses.Count);
        }

        [TestMethod]
        public void GetAvailableWarehouses_WithReservedWarehouse_ShouldReturnEmptyList()
        {
            var warehouse = _warehouseRepo.Add(_validWarehouse);
            var client = _userRepo.Add(_validClient);
    
            _reservationService.AddReservation(_validStartDate, _validEndDate, warehouse.WarehouseId, client.UserId);
    
            var unavailableStartDate = _validStartDate.AddDays(1);
            var unavailableEndDate = _validEndDate;
    
            var availableWarehouses = _reservationService.GetAvailableWarehouses(unavailableStartDate, unavailableEndDate);
    
            Assert.AreEqual(0, availableWarehouses.Count);
        }
    }
}
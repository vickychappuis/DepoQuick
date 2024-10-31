using DepoQuick.Backend.Dtos.Reservations;
using DepoQuick.Backend.Models;
using DepoQuick.Backend.Providers;
using DepoQuick.Backend.Repos;
using DepoQuick.Backend.Services;

namespace DepoQuick.Tests.Services
{
    [TestClass]
    public class ReservationService
    {
        private InMemoryDatabase _db;
        private ReservationRepo _reservationRepo;
        private WarehouseRepo _warehouseRepo;
        private PromotionRepo _promotionRepo;
        private UserRepo _userRepo;
        private Backend.Services.PromotionService _promotionService;
        private PriceService _priceService;
        private Backend.Services.ReservationService _reservationService;
        
        private DateTime _validStartDate = DateTime.Parse("2025-01-01");
        private DateTime _validEndDate = DateTime.Parse("2025-01-08");
        private Warehouse _validWarehouse = new Warehouse(WarehouseZone.A, WarehouseSize.Small, true);
        private User _validClient = new User("John Doe", "john@doe.com", "password", true);

        [TestInitialize]
        public void TestInit()
        {
            _db = new InMemoryDatabase();
            _reservationRepo = new ReservationRepo(_db);
            _warehouseRepo = new WarehouseRepo(_db);
            _promotionRepo = new PromotionRepo(_db);
            _promotionService = new Backend.Services.PromotionService(_promotionRepo);
            _userRepo = new UserRepo(_db);
            _priceService = new PriceService(_promotionService);
            _reservationService = new Backend.Services.ReservationService(_reservationRepo, _warehouseRepo, _userRepo, _priceService);
        }

        [TestMethod]
        public void AddReservation_WithValidData_ShouldAddToDatabase()
        {
            _userRepo.Add(_validClient);
            
            var reservation = _reservationService.AddReservation(_validStartDate, _validEndDate, _validWarehouse, _validClient.Id);
            
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
                _reservationService.AddReservation(afterDate, beforeDate, _validWarehouse, _validClient.Id));
        }
        
        [TestMethod]
        public void AddReservation_StartDateInPast_ShouldThrow()
        {
            DateTimeService.CurrentDateTime = DateTime.Parse("2023-01-01");
            _userRepo.Add(_validClient);
            
            var pastDate = DateTime.Parse("2021-01-01");
            var afterPastDate = DateTime.Parse("2021-01-03");
            
            Assert.ThrowsException<ArgumentException>(() =>
                _reservationService.AddReservation(pastDate, afterPastDate, _validWarehouse, _validClient.Id));
        }
        
        
            
        
        [TestMethod]
        public void AddReservation_WithOverlappingReservation_ShouldThrow()
        {
            _userRepo.Add(_validClient);
            _reservationService.AddReservation(_validStartDate, _validEndDate, _validWarehouse, _validClient.Id);
            
            Assert.ThrowsException<ArgumentException>(() =>
                _reservationService.AddReservation(_validStartDate, _validEndDate, _validWarehouse, _validClient.Id));
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
                WarehouseId = _validWarehouse.Id
            };
            
            var reservation = _reservationService.AddReservation(dto, _validClient.Id);
            
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
            
            Assert.ThrowsException<ArgumentNullException>(() => _reservationService.AddReservation(dto, _validClient.Id));
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
            Assert.ThrowsException<InvalidOperationException>(() => _reservationService.AddReservation(dto, _validClient.Id));
        }

        [TestMethod]
        public void GetAllClientReservations_GivenClientId_ShouldReturnClientReservations()
        {
            
            _userRepo.Add(_validClient);
            var reservation = _reservationService.AddReservation(_validStartDate, _validEndDate, _validWarehouse, _validClient.Id);
            
            var clientReservations = _reservationService.GetAllClientReservations(_validClient.Id);
            
            Assert.AreEqual(1, clientReservations.Count);
            Assert.AreEqual(reservation, clientReservations[0]);
        }
        
        [TestMethod]
        public void UpdateReservation_GivenApprovedDtoWithRejectionNote_ShouldThrow()
        {
            _userRepo.Add(_validClient);
            
            var reservation = _reservationService.AddReservation(_validStartDate, _validEndDate, _validWarehouse, _validClient.Id);
            var updateDto = new UpdateReservationDto
            {
                IsApproved = true,
                RejectionNote = "Note"
            };
            
            Assert.ThrowsException<Exception>(() => _reservationService.UpdateReservation(reservation.Id, updateDto));
        }
        
        [TestMethod]
        public void UpdateReservation_GivenValidDto_ShouldUpdateReservation()
        {
            _userRepo.Add(_validClient);
            
            var reservation = _reservationService.AddReservation(_validStartDate, _validEndDate, _validWarehouse, _validClient.Id);
            var updateDto = new UpdateReservationDto
            {
                IsApproved = true
            };
            
            _reservationService.UpdateReservation(reservation.Id, updateDto);
            
            Assert.AreEqual(ReservationStatus.Approved, reservation.Status);
        }
        
        [TestMethod]
        public void UpdateReservation_GivenValidDtoWithRejectionNote_ShouldUpdateReservation()
        {
            _userRepo.Add(_validClient);
            
            var reservation = _reservationService.AddReservation(_validStartDate, _validEndDate, _validWarehouse,  _validClient.Id);
            var updateDto = new UpdateReservationDto
            {
                IsApproved = false,
                RejectionNote = "Note"
            };
            
            _reservationService.UpdateReservation(reservation.Id, updateDto);
            
            Assert.AreEqual(ReservationStatus.Rejected, reservation.Status);
            Assert.AreEqual("Note", reservation.RejectionNote);
        }
        
        [TestMethod]
        public void UpdateReservation_GivenNonExistentId_ShouldThrow()
        {
            const int nonExistentReservationId = 1;
            
            var updateDto = new UpdateReservationDto
            {
                IsApproved = true,
            };
            
            Assert.ThrowsException<InvalidOperationException>(() => _reservationService.UpdateReservation(nonExistentReservationId, updateDto));
        }
        
        [TestMethod]
        public void UpdateReservation_GivenDtoWithRejectionLongerThan300Chars_ShouldThrow()
        {
            _userRepo.Add(_validClient);
            
            var reservation = _reservationService.AddReservation(_validStartDate, _validEndDate, _validWarehouse, _validClient.Id);
            var updateDto = new UpdateReservationDto
            {
                IsApproved = false,
                RejectionNote = new string('v', 301)
            };
            
            Assert.ThrowsException<ArgumentException>(() => _reservationService.UpdateReservation(reservation.Id, updateDto));
        }
        
        [TestMethod]
        public void GetAll_ShouldReturnAllReservations()
        {
            _userRepo.Add(_validClient);
            
            var reservation = _reservationService.AddReservation(_validStartDate, _validEndDate, _validWarehouse, _validClient.Id);
            
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
            
            Assert.ThrowsException<ArgumentException>(() => _reservationService.AddReservation(pastDate, _validEndDate, _validWarehouse, _validClient.Id));
        }
        
        [TestMethod]
        public void AddReservation_GivenNonExistentClient_ShouldThrow()
        {
            const int nonExistentClientId = 1;
            
            Assert.ThrowsException<InvalidOperationException>(() => _reservationService.AddReservation(_validStartDate, _validEndDate, _validWarehouse, nonExistentClientId));
        }
    }
}

using DepoQuick.Backend.Dtos.Reservations;
using DepoQuick.Models;
using DepoQuick.DataAccess.Repos;
using Microsoft.EntityFrameworkCore;

namespace DepoQuick.Backend.Services;

public class ReservationService
{
    private readonly IRepo<Reservation, int> _reservationRepo;
    private readonly IRepo<Warehouse, int> _warehouseRepo;
    private readonly PriceService _priceService;

    public ReservationService(IRepo<Reservation, int> reservationRepo, IRepo<Warehouse, int> warehouseRepo,
        PriceService priceService)
    {
        _warehouseRepo = warehouseRepo;
        _reservationRepo = reservationRepo;
        _priceService = priceService;
    }

    public Reservation AddReservation(DateTime startDate, DateTime endDate, int warehouseId, int clientId)
    {
        if (startDate > endDate)
            throw new ArgumentException("End date can't be before start date", nameof(startDate));

        if (startDate < DateTimeService.CurrentDateTime.Date)
            throw new ArgumentException("Start date can't be in the past", nameof(startDate));
        
        List<Reservation> warehouseReservations =
            Enumerable.Where(_reservationRepo.GetAll(), r => r.WarehouseId == warehouseId).ToList();

        Warehouse? warehouse = _warehouseRepo.Get(warehouseId);

        if (warehouse is null)
            throw new InvalidOperationException("Warehouse not found");
    
        if (warehouse.AvailableFrom > startDate || warehouse.AvailableTo < endDate)
            throw new ArgumentException("Warehouse not available at specified time");
        
        foreach (var reservation in warehouseReservations)
            if (endDate > reservation.StartDate && startDate < reservation.EndDate || (startDate == reservation.StartDate && endDate == reservation.EndDate))
                throw new ArgumentException("Reservation overlaps with existing reservation");

        double price = _priceService.CalculatePrice(warehouse.Size, warehouse.IsHeated, startDate, endDate);

        Reservation newReservation = new Reservation
        {
            StartDate = startDate,
            EndDate = endDate,
            WarehouseId = warehouseId,
            Price = price,
            Status = ReservationStatus.Pending,
            ClientId = clientId
        }; 
        
        try
        {
            return _reservationRepo.Add(newReservation);
        }
        catch (DbUpdateException)
        {
            throw new InvalidOperationException("Client not found");
        }
    }

    public Reservation AddReservation(AddReservationDto addReservationDto, int clientId)
    {
        if (addReservationDto.WarehouseId is null)
            throw new ArgumentNullException(nameof(addReservationDto.WarehouseId));

        return AddReservation(addReservationDto.StartDate, addReservationDto.EndDate,
            (int)addReservationDto.WarehouseId, clientId);
    }


    public List<Reservation> GetAll()
    {
        return _reservationRepo.GetAll();
    }

    public List<Reservation> GetAllClientReservations(int clientId)
    {
        return _reservationRepo.GetAll().Where(r => r.ClientId == clientId).ToList();
    }
    
    public List<Warehouse> GetAvailableWarehouses(DateTime startDate, DateTime endDate)
    {
        if (startDate > endDate)
            throw new ArgumentException("End date can't be before start date", nameof(startDate));
        
        List<Warehouse> availableWarehouses = new List<Warehouse>();
        
        List<Warehouse> warehouses = _warehouseRepo.GetAll();
        List<Reservation> reservations = _reservationRepo.GetAll();

        foreach (var warehouse in warehouses)
        {
            if (warehouse.AvailableFrom > startDate || warehouse.AvailableTo < endDate)
                continue;

            var isAvailable = !reservations.Exists(
                r => r.WarehouseId == warehouse.WarehouseId
                     && (endDate > r.StartDate && startDate < r.EndDate || (startDate == r.StartDate && endDate == r.EndDate))
            );

            if (isAvailable)
                availableWarehouses.Add(warehouse);
        }

        return availableWarehouses;
    }

    public Reservation UpdateReservationStatus(int id, UpdateReservationDto updateReservationDto)
    {
        if (updateReservationDto.IsApproved && updateReservationDto.RejectionNote is not null)
            throw new Exception("Rejection note can't be set if reservation is approved");

        const int maxRejectionNoteLength = 300;

        if (updateReservationDto.RejectionNote is not null &&
            updateReservationDto.RejectionNote.Length > maxRejectionNoteLength)
            throw new ArgumentException($"Rejection note can't be longer than {maxRejectionNoteLength} characters");
        
        Reservation? updatedReservation = _reservationRepo.Get(id);
        
        if (updatedReservation is null)
            throw new InvalidOperationException("Reservation not found");
        
        if (updateReservationDto.IsApproved)
        {
            updatedReservation.Status = ReservationStatus.Approved;
            updatedReservation.PaymentStatus = PaymentStatus.Captured;
        }
        else
        {
            updatedReservation.Status = ReservationStatus.Rejected;
            updatedReservation.PaymentStatus = null;
            updatedReservation.RejectionNote = updateReservationDto.RejectionNote;
        }
        
        return _reservationRepo.Update(updatedReservation);
    }
    public Reservation UpdateReservationPaymentStatus(int id, PaymentStatus paymentStatus)
    {
        Reservation? updatedReservation = _reservationRepo.Get(id);
        
        if (updatedReservation is null)
            throw new InvalidOperationException("Reservation not found");

        updatedReservation.PaymentStatus = paymentStatus;
        
        return _reservationRepo.Update(updatedReservation);
    }
}
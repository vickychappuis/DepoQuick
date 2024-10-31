using DepoQuick.Backend.Dtos.Reservations;
using DepoQuick.Backend.Models;
using DepoQuick.Backend.Providers;

namespace DepoQuick.Backend.Repos;

public class ReservationRepo : IRepo<Reservation>
{
    private InMemoryDatabase _db;

    public ReservationRepo(InMemoryDatabase database)
    {
        _db = database;
    }
    
    public void Add(Reservation reservation)
    {
        _db.Reservations.Add(reservation);
    }
    
    public List<Reservation> GetAll()
    {
        return _db.Reservations;
    }
    
    public Reservation Update(int id, UpdateReservationDto updateReservationDto)
    {
        var existingReservation = _db.Reservations.FirstOrDefault(r => r.Id == id);
        
        if (existingReservation is null)
            throw new InvalidOperationException("Reservation not found");
        
        existingReservation.Status = updateReservationDto.IsApproved 
                ? ReservationStatus.Approved 
                : ReservationStatus.Rejected;
        if (updateReservationDto.RejectionNote is not null && !updateReservationDto.IsApproved)
            existingReservation.RejectionNote = updateReservationDto.RejectionNote;
        return existingReservation;
    }
}
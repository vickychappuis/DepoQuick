using DepoQuick.Models;
using Microsoft.EntityFrameworkCore;

namespace DepoQuick.DataAccess.Repos;

public class ReservationRepo : IRepo<Reservation, int>
{
    private readonly IDbContextFactory<Context> _contextFactory;
    
    public ReservationRepo(IDbContextFactory<Context> context)
    {
        _contextFactory = context;
    }
    
    public Reservation Add(Reservation reservation)
    {
        using var context = _contextFactory.CreateDbContext();
        var reservationEntry = context.Reservations.Add(reservation);
        context.SaveChanges();
        
        return reservationEntry.Entity;
    }
    
    public List<Reservation> GetAll()
    {
        using var context = _contextFactory.CreateDbContext();
        return context.Reservations.Include(r => r.Warehouse).ToList();
    }

    public Reservation? Get(int id)
    {
        using var context = _contextFactory.CreateDbContext();
        return context.Reservations.Find(id);
    }

    public void Delete(int id)
    {
        using var context = _contextFactory.CreateDbContext();
        var reservation = context.Reservations.Find(id);
        
        if (reservation is null)
            return;
        
        context.Reservations.Remove(reservation);
        context.SaveChanges();
    }

    public Reservation Update(Reservation entity)
    {
        using var context = _contextFactory.CreateDbContext();
        var reservationEntry = context.Reservations.Update(entity);
        context.SaveChanges();
        
        return reservationEntry.Entity;
    }
}
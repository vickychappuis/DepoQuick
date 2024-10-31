using DepoQuick.Backend.Models;

namespace DepoQuick.Backend.Providers;

public class InMemoryDatabase
{
    public List<User> Users { get; }
    public List<Warehouse> Warehouses { get; set; }
    public List<Promotion> Promotions { get; set; }
    public List<Reservation> Reservations { get; set; }
    
    public InMemoryDatabase()
    {
        Warehouses = new List<Warehouse>();
        Promotions = new List<Promotion>();
        Users = new List<User>();
        Reservations = new List<Reservation>();
    }
}
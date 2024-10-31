using DepoQuick.Models;

using Microsoft.EntityFrameworkCore;

namespace DepoQuick.DataAccess;
public class Context : DbContext {
    public DbSet<User> Users { get; set; }
    public DbSet<Warehouse> Warehouses { get; set; }
    public DbSet<Promotion> Promotions { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    
    public Context(DbContextOptions<Context> options) : base(options)
    {
        var relationalOptionsExtension = options.Extensions
            .OfType<Microsoft.EntityFrameworkCore.Infrastructure.RelationalOptionsExtension>()
            .FirstOrDefault();
        
        var databaseType = relationalOptionsExtension?.Connection?.GetType().Name;
        if( databaseType != null && databaseType.Contains("Sqlite"))
            Database.EnsureCreated();
        else
            Database.Migrate();
    }
}
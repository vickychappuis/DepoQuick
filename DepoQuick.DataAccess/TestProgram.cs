using DepoQuick.DataAccess.Repos;
using DepoQuick.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DepoQuick.DataAccess;

public class TestProgram
{
    public readonly ServiceProvider ServiceProvider;
    
    public TestProgram()
    {
        var inMemoryDatabase = new SqliteConnection("Data Source=:memory:");
        inMemoryDatabase.Open();

        var services = new ServiceCollection();

        services.AddDbContextFactory<Context>(options => options.UseSqlite(inMemoryDatabase));
        
        services.AddSingleton<IRepo<User, int>, UserRepo>();
        services.AddSingleton<IRepo<User, string>, UserRepo>();
        services.AddSingleton<IRepo<Promotion, int>, PromotionRepo>();
        services.AddSingleton<IRepo<Warehouse, int>, WarehouseRepo>();
        services.AddSingleton<IRepo<Reservation, int>, ReservationRepo>();
        
        ServiceProvider = services.BuildServiceProvider();
    }
}
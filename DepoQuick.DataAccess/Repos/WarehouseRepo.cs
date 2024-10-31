using DepoQuick.Models;
using Microsoft.EntityFrameworkCore;

namespace DepoQuick.DataAccess.Repos;

public class WarehouseRepo : IRepo<Warehouse, int>

{
    private readonly IDbContextFactory<Context> _contextFactory;

    public WarehouseRepo(IDbContextFactory<Context> context)
    {
        _contextFactory = context;
    }
    
    public Warehouse Add(Warehouse warehouse)
    {
        using var context = _contextFactory.CreateDbContext();
        var warehouseEntry = context.Warehouses.Add(warehouse);
        context.SaveChanges();
        
        return warehouseEntry.Entity;
    }

    public List<Warehouse> GetAll()
    {
        using var context = _contextFactory.CreateDbContext();
        return context.Warehouses.ToList();
    }
    
    public Warehouse? Get(int id)
    {
        using var context = _contextFactory.CreateDbContext();
        return context.Warehouses.Find(id);
    }

    public void Delete(int id)
    {
        using var context = _contextFactory.CreateDbContext();
        var warehouse = context.Warehouses.Find(id);

        if (warehouse is null)
            return;

        context.Warehouses.Remove(warehouse);
        context.SaveChanges();
    }

    public Warehouse Update(Warehouse entity)
    {
        using var context = _contextFactory.CreateDbContext();
        var warehouseEntry = context.Warehouses.Update(entity);
        context.SaveChanges();
        
        return warehouseEntry.Entity;
    }
}

using DepoQuick.Backend.Models;
using DepoQuick.Backend.Providers;

namespace DepoQuick.Backend.Repos;

public class WarehouseRepo : IRepo<Warehouse>

{
    private InMemoryDatabase _db;

    public WarehouseRepo(InMemoryDatabase database)
    {
        _db = database;
    }
    
    public void Add(Warehouse warehouse)
    {
        _db.Warehouses.Add(warehouse);
    }

    public List<Warehouse> GetAll()
    {
        return _db.Warehouses;
    }
    
    public Warehouse? Get(int id)
    {
        return _db.Warehouses.Find(w => w.Id == id);
    }

    public void Delete(int id)
    {
        var warehouse = Get(id);
        _db.Warehouses.Remove(warehouse);
    }
    
}

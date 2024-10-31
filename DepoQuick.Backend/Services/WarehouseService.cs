using DepoQuick.Backend.Dtos.Warehouses;
using DepoQuick.Models;
using DepoQuick.DataAccess.Repos;
using WarehouseZone = DepoQuick.Models.WarehouseZone;

namespace DepoQuick.Backend.Services;

public class WarehouseService
{
    private readonly IRepo<Warehouse, int> _warehouseRepo;
    private IRepo<Reservation, int> _reservationRepo;
    
    public WarehouseService(IRepo<Warehouse, int> warehouseRepo, IRepo<Reservation, int> reservationRepo)
    {
        _warehouseRepo = warehouseRepo;
        _reservationRepo = reservationRepo;
    }

    
    public Warehouse AddWarehouse(string name, WarehouseZone zone, WarehouseSize size, bool isHeated, DateTime availableFrom, DateTime availableTo)
    {
        if (availableFrom > availableTo)
            throw new ArgumentException("Available to date can't be before available from", nameof(availableTo));
        
        Warehouse newWarehouse = new Warehouse(name, zone, size, isHeated, availableFrom, availableTo);
        
        _warehouseRepo.Add(newWarehouse);
        
        return newWarehouse;
    }

    public Warehouse AddWarehouse(AddWarehouseDto addWarehouseDto)
    {
        return AddWarehouse(addWarehouseDto.Name, addWarehouseDto.Zone, addWarehouseDto.Size, addWarehouseDto.IsHeated, addWarehouseDto.AvailableFrom, addWarehouseDto.AvailableTo);
    }

    public List<Warehouse> GetAllWarehouses()
    {
        return _warehouseRepo.GetAll();
    }
    
    public void DeleteWarehouse(int id)
    {
        int WarehouseWithActiveReservation = Enumerable.Where<Reservation>(_reservationRepo.GetAll(), r =>
            r.WarehouseId == id && (r.Status == ReservationStatus.Approved || r.Status == ReservationStatus.Pending)).Count();
        
        if (WarehouseWithActiveReservation > 0)    
            throw new InvalidOperationException("Warehouse has reservations approved or pending");
        else
            _warehouseRepo.Delete(id);
    }
}
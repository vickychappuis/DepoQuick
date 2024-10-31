using System.ComponentModel.DataAnnotations;
using DepoQuick.Models;

namespace DepoQuick.Backend.Dtos.Warehouses;

public class AddWarehouseDto
{
    [Required]
    public string Name { get; set; }
    
    [Required]
    public WarehouseZone Zone { get; set; }
    
    [Required]
    public WarehouseSize Size { get; set; }
    
    [Required]
    public bool IsHeated { get; set; }
    
    [Required]
    public DateTime AvailableFrom { get; set; }
    
    [Required]
    public DateTime AvailableTo { get; set; }
}
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace DepoQuick.Models
{
    public enum WarehouseZone
    {
        A = 'A',
        B = 'B',
        C = 'C',
        D = 'D',
        E = 'E'
    }

    public enum WarehouseSize
    {
        Small,
        Medium,
        Large,
    }
    
    public class Warehouse
    {
        [Key, Range(0, int.MaxValue)]
        public int WarehouseId { get; set; }
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
        
        public Warehouse(string name, WarehouseZone zone, WarehouseSize size, bool isHeated, DateTime availableFrom, DateTime availableTo)
        {
            Name = name;
            Zone = zone;
            Size = size;
            IsHeated = isHeated;
            AvailableFrom = availableFrom;
            AvailableTo = availableTo;
        }
        
        public override bool Equals(object? obj)
        {
            var warehouse = obj as Warehouse;
            
            if (warehouse is null)
                return false;
            
            return WarehouseId == warehouse.WarehouseId && Zone == warehouse.Zone && Size == warehouse.Size &&
                   IsHeated == warehouse.IsHeated;
        }

        public override string ToString()
        {
            var heated = IsHeated ? "Heated" : "Not heated";

            return $"{Name} ({WarehouseId}) {Size} - {Zone} - {heated}";
        }
    }
}

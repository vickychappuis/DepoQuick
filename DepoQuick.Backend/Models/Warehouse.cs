namespace DepoQuick.Backend.Models
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
        public static int nextWarehouseId = 0;
        
        public int Id { get; }
        public WarehouseZone Zone { get; set; }
        public WarehouseSize Size { get; set; }
        public bool IsHeated { get; set; }
        
        public Warehouse(WarehouseZone zone, WarehouseSize size, bool isHeated)
        {
            Id = nextWarehouseId++;
            Zone = zone;
            Size = size;
            IsHeated = isHeated;
        }
    }
}

using System.ComponentModel.DataAnnotations;

namespace DepoQuick.Backend.Dtos.Reservations;

public class AddReservationDto
{
    [Required]
    public DateTime StartDate { get; set; }
    
    [Required]
    public DateTime EndDate { get; set; }
    
    [Required]
    public int? WarehouseId { get; set; }
}
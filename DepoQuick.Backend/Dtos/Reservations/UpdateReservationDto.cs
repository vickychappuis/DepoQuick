using System.ComponentModel.DataAnnotations;

namespace DepoQuick.Backend.Dtos.Reservations;

public class UpdateReservationDto
{
    public bool IsApproved { get; set; }

    [MaxLength(300)]
    public string? RejectionNote { get; set; }
}
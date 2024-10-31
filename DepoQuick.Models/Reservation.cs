using System.ComponentModel.DataAnnotations;

namespace DepoQuick.Models
{
    public enum ReservationStatus
    {
        Pending,
        Approved,
        Rejected
    }
    public enum PaymentStatus
    {
        Reserved,
        Captured
    }
    public class Reservation
    {
        [Key, Range(0, int.MaxValue)]
        public int ReservationId { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        [Range(0, int.MaxValue)]
        public int WarehouseId { get; set; }
        [Required]
        public Warehouse Warehouse { get; set; }
        [Required, Range(0, double.MaxValue)]
        public double Price { get; set; }
        [Required]
        public ReservationStatus Status { get; set; }
        public string? RejectionNote { get; set; }
        public PaymentStatus? PaymentStatus { get; set; }
        [Required, Range(0, int.MaxValue)]
        public int ClientId { get; set; }
        [Required]
        public User Client { get; set; }

        public override bool Equals(object? obj)
        {
            var reservation = obj as Reservation;
            if (reservation == null) return false;

            return ReservationId == reservation.ReservationId && StartDate == reservation.StartDate && EndDate == reservation.EndDate && WarehouseId == reservation.WarehouseId && Price == reservation.Price && Status == reservation.Status && RejectionNote == reservation.RejectionNote && PaymentStatus == reservation.PaymentStatus && ClientId == reservation.ClientId;
        }
    }
}

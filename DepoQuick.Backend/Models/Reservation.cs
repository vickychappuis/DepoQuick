namespace DepoQuick.Backend.Models
{
    public enum ReservationStatus
    {
        Pending,
        Approved,
        Rejected
    }

    public class Reservation
    {
        private static int NextReservationId = 0;
        public int Id { get; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Warehouse Warehouse { get; set; }
        public double Price { get; set; }
        public ReservationStatus Status { get; set; }
        public string? RejectionNote { get; set; }
        public User Client { get; set; }
       
       public Reservation(DateTime startDate, DateTime endDate, Warehouse warehouse, double price, ReservationStatus status, string? rejectionNote, User client)
       {
           Id = NextReservationId++;
           StartDate = startDate;
           EndDate = endDate;
           Warehouse = warehouse;
           Price = price;
           Status = status;
           RejectionNote = rejectionNote;
           Client = client;
       }
    }
}

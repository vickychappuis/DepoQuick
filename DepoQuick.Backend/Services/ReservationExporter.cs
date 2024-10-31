using System.Text;
using DepoQuick.Models;

namespace DepoQuick.Backend.Services;

public interface IExportStrategy<T>
{
    string Export(List<T> data);
}

public class CSVExportStrategy : IExportStrategy<Reservation>
{
    public string Export(List<Reservation> reservations)
    {
        var csv = new StringBuilder();
        csv.Append("DEPOSITO,RESERVA,PAGO\n");
        foreach (var reservation in reservations)
        {
            var warehouseId = reservation.Warehouse!.WarehouseId;
            var reservationId = reservation.ReservationId;
            var paymentStatus = reservation.PaymentStatus == null ? "nulo" : reservation.PaymentStatus.ToString();              
            csv.Append($"{warehouseId},{reservationId},{paymentStatus}\n");
        }

        var fileBytes = Encoding.UTF8.GetBytes(csv.ToString());
        var contentType = "text/csv";

        return $"data:{contentType};base64,{Convert.ToBase64String(fileBytes)}";
    }
}

public class TSVExportStrategy : IExportStrategy<Reservation>
{
    public string Export(List<Reservation> reservations)
    {
        var tsv = new StringBuilder();
        tsv.Append("DEPOSITO\tRESERVA\tPAGO\n");
        foreach (var reservation in reservations)
        {
            var warehouseId = reservation.Warehouse!.WarehouseId;
            var reservationId = reservation.ReservationId;
            var paymentStatus = reservation.PaymentStatus == null ? "nulo" : reservation.PaymentStatus.ToString();              
            
            tsv.Append($"{warehouseId}\t{reservationId}\t{paymentStatus}\n");
        }
        
        var fileBytes = Encoding.UTF8.GetBytes(tsv.ToString());
        var contentType = "text/tab-separated-values";

        return $"data:{contentType};base64,{Convert.ToBase64String(fileBytes)}";
    }
}

public class ReservationExporter
{
    private IExportStrategy<Reservation> _exportStrategy;

    public ReservationExporter(IExportStrategy<Reservation> exportStrategy)
    {
        _exportStrategy = exportStrategy;
    }

    public string ExportReservations(List<Reservation> reservations)
    {
        return _exportStrategy.Export(reservations);
    }
}

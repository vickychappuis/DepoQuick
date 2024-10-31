namespace DepoQuick.Backend.Services;

public class DateTimeService
{
    private static DateTime? _currentDateTime;

    public static DateTime CurrentDateTime
    {
        get => _currentDateTime ?? DateTime.Now;
        set => _currentDateTime = value;
    }
}
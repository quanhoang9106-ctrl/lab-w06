
namespace LabW06;

public class BorrowingException : Exception
{
    public BorrowingException(string message) : base(message) { }
    public BorrowingException(string message, Exception innerException)
        : base(message, innerException) { }
}

public class DeviceUnavailableException : BorrowingException
{
    public string DeviceId { get; }
    public DeviceStatus CurrentStatus { get; }

    public DeviceUnavailableException(string deviceId, DeviceStatus status)
        : base($"Thiet bi {deviceId} khong san sang (trang thai: {status}).")
    {
        DeviceId = deviceId;
        CurrentStatus = status;
    }
}

public class BorrowLimitExceededException : BorrowingException
{
    public string BorrowerId { get; }
    public int MaximumDevices { get; }

    public BorrowLimitExceededException(string borrowerId, int maximumDevices)
        : base($"Nguoi muon {borrowerId} chi duoc muon toi da {maximumDevices} thiet bi.")
    {
        BorrowerId = borrowerId;
        MaximumDevices = maximumDevices;
    }
}

public class InvalidBorrowPeriodException : BorrowingException
{
    public string DeviceId { get; }
    public int RequestedDays { get; }
    public int MaximumDays { get; }

    public InvalidBorrowPeriodException(string deviceId, int requestedDays, int maximumDays)
        : base($"Thoi han {requestedDays} ngay khong hop le cho {deviceId} (toi da {maximumDays} ngay).")
    {
        DeviceId = deviceId;
        RequestedDays = requestedDays;
        MaximumDays = maximumDays;
    }
}

public class DuplicateDeviceException : BorrowingException
{
    public DuplicateDeviceException(string deviceId)
        : base($"Thiet bi {deviceId} da ton tai trong danh sach.") { }
}

public class DeviceNotInSlipException : BorrowingException
{
    public DeviceNotInSlipException(string deviceId)
        : base($"Thiet bi {deviceId} khong thuoc phieu muon.") { }
}

public class DeviceAlreadyReturnedException : BorrowingException
{
    public DeviceAlreadyReturnedException(string deviceId)
        : base($"Thiet bi {deviceId} da duoc tra truoc do.") { }
}

public class InvalidReturnDateException : BorrowingException
{
    public InvalidReturnDateException(DateTime borrowDate, DateTime returnDate)
        : base($"Ngay tra {returnDate:dd/MM/yyyy} truoc ngay muon {borrowDate:dd/MM/yyyy}.") { }
}

public class InvalidLessonPeriodException : BorrowingException
{
    public InvalidLessonPeriodException(int fromPeriod, int toPeriod, int maximumPeriods)
        : base($"Khoang tiet {fromPeriod}-{toPeriod} khong hop le (toi da {maximumPeriods} tiet).") { }
}

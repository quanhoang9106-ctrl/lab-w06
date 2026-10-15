
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

    public DeviceUnavailableException(string deviceId, DeviceStatus currentStatus)
        : base($"Thiet bi {deviceId} khong san sang: {currentStatus}")
    {
        DeviceId = deviceId;
        CurrentStatus = currentStatus;
    }
}

public class BorrowLimitExceededException : BorrowingException
{
    public string BorrowerId { get; }
    public int MaximumDevices { get; }

    public BorrowLimitExceededException(string borrowerId, int maximumDevices)
        : base($"Nguoi muon {borrowerId} vuot gioi han {maximumDevices} thiet bi")
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
        : base($"Thoi han muon {requestedDays} ngay khong hop le cho {deviceId}")
    {
        DeviceId = deviceId;
        RequestedDays = requestedDays;
        MaximumDays = maximumDays;
    }
}

public class DuplicateDeviceException : BorrowingException
{
    public DuplicateDeviceException(string deviceId)
        : base($"Thiet bi {deviceId} da co trong phieu") { }
}

public class DeviceNotInSlipException : BorrowingException
{
    public DeviceNotInSlipException(string deviceId)
        : base($"Khong tim thay thiet bi {deviceId} trong phieu") { }
}

public class DeviceAlreadyReturnedException : BorrowingException
{
    public DeviceAlreadyReturnedException(string deviceId)
        : base($"Thiet bi {deviceId} da duoc tra") { }
}

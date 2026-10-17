
namespace LabW06;

public class BorrowingException : Exception
{
    public BorrowingException(string message) : base(message) { }

    public BorrowingException(string message, Exception innerException)
        : base(message, innerException) { }
}

public sealed class DeviceUnavailableException : BorrowingException
{
    public string DeviceId { get; }
    public DeviceStatus CurrentStatus { get; }

    public DeviceUnavailableException(string deviceId, DeviceStatus status)
        : base($"Thiết bị {deviceId} không sẵn sàng (trạng thái: {status}).")
    {
        DeviceId = deviceId;
        CurrentStatus = status;
    }
}

public sealed class BorrowLimitExceededException : BorrowingException
{
    public string BorrowerId { get; }
    public int MaximumDevices { get; }

    public BorrowLimitExceededException(string borrowerId, int maximumDevices)
        : base($"Người mượn {borrowerId} chỉ được mượn tối đa {maximumDevices} thiết bị.")
    {
        BorrowerId = borrowerId;
        MaximumDevices = maximumDevices;
    }
}

public sealed class InvalidBorrowPeriodException : BorrowingException
{
    public string DeviceId { get; }
    public int RequestedDays { get; }
    public int MaximumDays { get; }

    public InvalidBorrowPeriodException(string deviceId, int requestedDays, int maximumDays)
        : base($"Thời hạn {requestedDays} ngày không hợp lệ cho {deviceId} (tối đa {maximumDays} ngày).")
    {
        DeviceId = deviceId;
        RequestedDays = requestedDays;
        MaximumDays = maximumDays;
    }
}

// TỐI ƯU: Lưu mã thiết bị vào thuộc tính để tiện kiểm tra lỗi.
public sealed class DuplicateDeviceException : BorrowingException
{
    public string DeviceId { get; }

    public DuplicateDeviceException(string deviceId)
        : base($"Thiết bị {deviceId} đã có trong danh sách.")
    {
        DeviceId = deviceId;
    }
}

public sealed class DeviceNotInSlipException : BorrowingException
{
    public string DeviceId { get; }

    public DeviceNotInSlipException(string deviceId)
        : base($"Thiết bị {deviceId} không thuộc phiếu mượn.")
    {
        DeviceId = deviceId;
    }
}

public sealed class DeviceAlreadyReturnedException : BorrowingException
{
    public string DeviceId { get; }

    public DeviceAlreadyReturnedException(string deviceId)
        : base($"Thiết bị {deviceId} đã được trả trước đó.")
    {
        DeviceId = deviceId;
    }
}

public sealed class InvalidReturnDateException : BorrowingException
{
    public DateTime BorrowedDate { get; }
    public DateTime ReturnedDate { get; }

    public InvalidReturnDateException(DateTime borrowedDate, DateTime returnedDate)
        : base($"Ngày trả {returnedDate:dd/MM/yyyy} trước ngày mượn {borrowedDate:dd/MM/yyyy}.")
    {
        BorrowedDate = borrowedDate;
        ReturnedDate = returnedDate;
    }
}

public sealed class InvalidLessonPeriodException : BorrowingException
{
    public int FromPeriod { get; }
    public int ToPeriod { get; }
    public int MaximumPeriods { get; }

    public InvalidLessonPeriodException(int fromPeriod, int toPeriod, int maximumPeriods)
        : base($"Khoảng tiết {fromPeriod}-{toPeriod} không hợp lệ (tối đa {maximumPeriods} tiết).")
    {
        FromPeriod = fromPeriod;
        ToPeriod = toPeriod;
        MaximumPeriods = maximumPeriods;
    }
}

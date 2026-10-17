
namespace LabW06;

public class BorrowItem
{
    public TeachingDevice Device { get; }
    public DateTime BorrowedDate { get; }
    public DateTime DueDate { get; }
    public int FromPeriod { get; }
    public int ToPeriod { get; }
    public DateTime? ReturnedDate { get; private set; }

    public bool IsReturned => ReturnedDate.HasValue;

    public BorrowItem(TeachingDevice device, DateTime borrowedDate, DateTime dueDate,
        int fromPeriod, int toPeriod)
    {
        ArgumentNullException.ThrowIfNull(device);

        if (dueDate.Date < borrowedDate.Date)
            throw new ArgumentException("Hạn trả không được trước ngày mượn.", nameof(dueDate));

        if (fromPeriod < 1 || toPeriod < fromPeriod ||
            (long)toPeriod - fromPeriod + 1 > device.MaxBorrowPeriods)
            throw new InvalidLessonPeriodException(fromPeriod, toPeriod, device.MaxBorrowPeriods);

        Device = device;
        BorrowedDate = borrowedDate.Date;
        DueDate = dueDate.Date;
        FromPeriod = fromPeriod;
        ToPeriod = toPeriod;
    }

    public void Return(DateTime returnedDate)
    {
        if (IsReturned)
            throw new DeviceAlreadyReturnedException(Device.DeviceId);

        if (returnedDate.Date < BorrowedDate)
            throw new InvalidReturnDateException(BorrowedDate, returnedDate);

        if (Device.Status != DeviceStatus.Borrowed)
            throw new DeviceUnavailableException(Device.DeviceId, Device.Status);

        // TỐI ƯU: Xử lý thay đổi trạng thái và ngày trả tại cùng một nơi.
        // Chỉ cập nhật sau khi kiểm tra toàn bộ điều kiện.
        Device.MarkAsAvailable();
        ReturnedDate = returnedDate.Date;
    }
}

public class BorrowingSlip
{
    // Mức phí minh họa vì đề chưa đưa ra công thức.
    private const decimal LateFeePerDay = 10000m;

    private readonly List<BorrowItem> _items = new();
    private readonly IReadOnlyList<BorrowItem> _readOnlyItems;

    public string SlipId { get; }
    public IBorrower Borrower { get; }
    public string TeachingRoom { get; }
    public DateTime BorrowedDate { get; }

    // TỐI ƯU: Chỉ tạo bộ bao danh sách một lần.
    // Không cho phép bên ngoài thêm/xóa item trực tiếp.
    public IReadOnlyList<BorrowItem> Items => _readOnlyItems;

    public BorrowingSlip(string slipId, IBorrower borrower,
        string teachingRoom, DateTime borrowedDate)
    {
        if (string.IsNullOrWhiteSpace(slipId))
            throw new ArgumentException("Mã phiếu không hợp lệ.", nameof(slipId));

        ArgumentNullException.ThrowIfNull(borrower);

        if (string.IsNullOrWhiteSpace(teachingRoom))
            throw new ArgumentException("Mã phòng không hợp lệ.", nameof(teachingRoom));

        SlipId = slipId.Trim();
        Borrower = borrower;
        TeachingRoom = teachingRoom.Trim();
        BorrowedDate = borrowedDate.Date;
        _readOnlyItems = _items.AsReadOnly();
    }

    public void AddDevice(TeachingDevice device, int borrowDays)
        => AddDevice(device, borrowDays, 1, 1);

    public void AddDevice(TeachingDevice device, int borrowDays,
        int fromPeriod, int toPeriod)
    {
        // TỐI ƯU: Kiểm tra trước, thay đổi trạng thái sau.
        ValidateBorrowing(device, borrowDays);

        var item = new BorrowItem(
            device,
            BorrowedDate,
            BorrowedDate.AddDays(borrowDays),
            fromPeriod,
            toPeriod
        );

        device.MarkAsBorrowed();

        try
        {
            _items.Add(item);
        }
        catch
        {
            // TỐI ƯU: Khôi phục trạng thái nếu thêm danh sách thất bại.
            device.MarkAsAvailable();
            throw;
        }
    }

    // TỐI ƯU: Tách riêng kiểm tra điều kiện để phương thức AddDevice dễ đọc.
    private void ValidateBorrowing(TeachingDevice device, int borrowDays)
    {
        ArgumentNullException.ThrowIfNull(device);

        if (borrowDays <= 0 || borrowDays > device.MaxBorrowDays)
            throw new InvalidBorrowPeriodException(
                device.DeviceId, borrowDays, device.MaxBorrowDays);

        if (_items.Any(item => string.Equals(
            item.Device.DeviceId,
            device.DeviceId,
            StringComparison.OrdinalIgnoreCase)))
            throw new DuplicateDeviceException(device.DeviceId);

        if (_items.Count >= Borrower.GetMaximumDevices())
            throw new BorrowLimitExceededException(
                Borrower.BorrowerId, Borrower.GetMaximumDevices());

        if (device.Status != DeviceStatus.Available)
            throw new DeviceUnavailableException(device.DeviceId, device.Status);
    }

    public decimal ReturnDevice(string deviceId, DateTime returnedDate)
    {
        if (string.IsNullOrWhiteSpace(deviceId))
            throw new ArgumentException("Mã thiết bị không hợp lệ.", nameof(deviceId));

        BorrowItem? item = _items.FirstOrDefault(i =>
            string.Equals(
                i.Device.DeviceId,
                deviceId.Trim(),
                StringComparison.OrdinalIgnoreCase));

        if (item is null)
            throw new DeviceNotInSlipException(deviceId);

        // TỐI ƯU: BorrowItem chịu trách nhiệm xử lý trả thiết bị.
        item.Return(returnedDate);

        return CalculateLateFee(item);
    }

    public decimal CalculateTotalLateFee()
        => _items.Sum(CalculateLateFee);

    // TỐI ƯU: Dùng một hàm chung tính phí trả muộn.
    private static decimal CalculateLateFee(BorrowItem item)
    {
        if (item.ReturnedDate is not DateTime returnedDate)
            return 0m;

        int lateDays = Math.Max(0,
            (returnedDate.Date - item.DueDate.Date).Days);

        return lateDays * LateFeePerDay;
    }
}

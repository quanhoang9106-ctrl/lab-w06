
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
            throw new ArgumentException("Han tra khong duoc truoc ngay muon.", nameof(dueDate));
        if (fromPeriod < 1 || toPeriod < fromPeriod ||
            toPeriod - (long)fromPeriod + 1 > device.MaxBorrowPeriods)
            throw new InvalidLessonPeriodException(fromPeriod, toPeriod, device.MaxBorrowPeriods);

        Device = device;
        BorrowedDate = borrowedDate.Date;
        DueDate = dueDate.Date;
        FromPeriod = fromPeriod;
        ToPeriod = toPeriod;
        ReturnedDate = null;
    }

    public void Return(DateTime returnedDate)
    {
        if (IsReturned)
            throw new DeviceAlreadyReturnedException(Device.DeviceId);
        if (returnedDate.Date < BorrowedDate.Date)
            throw new InvalidReturnDateException(BorrowedDate, returnedDate);

        ReturnedDate = returnedDate.Date;
    }
}

public class BorrowingSlip
{
    private const decimal LateFeePerDay = 10000m; // Muc phi minh hoa
    private readonly List<BorrowItem> _items = new();

    public string SlipId { get; }
    public IBorrower Borrower { get; }
    public string TeachingRoom { get; }
    public DateTime BorrowedDate { get; }
    public IReadOnlyList<BorrowItem> Items => _items.AsReadOnly();

    public BorrowingSlip(string slipId, IBorrower borrower,
        string teachingRoom, DateTime borrowedDate)
    {
        if (string.IsNullOrWhiteSpace(slipId))
            throw new ArgumentException("Ma phieu khong hop le.", nameof(slipId));
        ArgumentNullException.ThrowIfNull(borrower);
        if (string.IsNullOrWhiteSpace(teachingRoom))
            throw new ArgumentException("Ma phong khong hop le.", nameof(teachingRoom));

        SlipId = slipId.Trim();
        Borrower = borrower;
        TeachingRoom = teachingRoom.Trim();
        BorrowedDate = borrowedDate.Date;
    }

    public void AddDevice(TeachingDevice device, int borrowDays)
    {
        AddDevice(device, borrowDays, 1, 1);
    }

    public void AddDevice(TeachingDevice device, int borrowDays, int fromPeriod, int toPeriod)
    {
        ArgumentNullException.ThrowIfNull(device);
        if (borrowDays <= 0 || borrowDays > device.MaxBorrowDays)
            throw new InvalidBorrowPeriodException(device.DeviceId, borrowDays, device.MaxBorrowDays);
        if (_items.Any(i => string.Equals(i.Device.DeviceId, device.DeviceId,
            StringComparison.OrdinalIgnoreCase)))
            throw new DuplicateDeviceException(device.DeviceId);
        if (_items.Count >= Borrower.GetMaximumDevices())
            throw new BorrowLimitExceededException(Borrower.BorrowerId, Borrower.GetMaximumDevices());
        if (device.Status != DeviceStatus.Available)
            throw new DeviceUnavailableException(device.DeviceId, device.Status);

        // Tao item truoc, chi thay doi trang thai khi moi dieu kien hop le.
        var item = new BorrowItem(device, BorrowedDate,
            BorrowedDate.AddDays(borrowDays), fromPeriod, toPeriod);
        device.MarkAsBorrowed();
        try
        {
            _items.Add(item);
        }
        catch
        {
            device.MarkAsAvailable();
            throw;
        }
    }

    public decimal ReturnDevice(string deviceId, DateTime returnedDate)
    {
        if (string.IsNullOrWhiteSpace(deviceId))
            throw new ArgumentException("Ma thiet bi khong hop le.", nameof(deviceId));

        BorrowItem? item = _items.FirstOrDefault(i =>
            string.Equals(i.Device.DeviceId, deviceId.Trim(), StringComparison.OrdinalIgnoreCase));
        if (item == null)
            throw new DeviceNotInSlipException(deviceId);
        if (item.IsReturned)
            throw new DeviceAlreadyReturnedException(deviceId);
        if (returnedDate.Date < item.BorrowedDate.Date)
            throw new InvalidReturnDateException(item.BorrowedDate, returnedDate);
        if (item.Device.Status != DeviceStatus.Borrowed)
            throw new DeviceUnavailableException(deviceId, item.Device.Status);

        decimal fee = Math.Max(0, (returnedDate.Date - item.DueDate.Date).Days) * LateFeePerDay;
        item.Return(returnedDate);
        item.Device.MarkAsAvailable();
        return fee;
    }

    public decimal CalculateTotalLateFee()
    {
        return _items.Where(i => i.IsReturned)
            .Sum(i => Math.Max(0, (i.ReturnedDate!.Value.Date - i.DueDate.Date).Days) * LateFeePerDay);
    }
}


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
        Device = device;
        BorrowedDate = borrowedDate;
        DueDate = dueDate;
        FromPeriod = fromPeriod;
        ToPeriod = toPeriod;
    }

    public void Return(DateTime returnedDate)
    {
        ReturnedDate = returnedDate;
    }
}

public class BorrowingSlip
{
    private readonly List<BorrowItem> _items = new();

    public string SlipId { get; }
    public IBorrower Borrower { get; }
    public string TeachingRoom { get; }
    public DateTime BorrowedDate { get; }
    public IReadOnlyList<BorrowItem> Items => _items.AsReadOnly();

    public BorrowingSlip(string slipId, IBorrower borrower, string teachingRoom, DateTime borrowedDate)
    {
        SlipId = slipId;
        Borrower = borrower;
        TeachingRoom = teachingRoom;
        BorrowedDate = borrowedDate;
    }

    public void AddDevice(TeachingDevice device, int borrowDays)
    {
        if (device == null)
            throw new ArgumentNullException(nameof(device));

        if (borrowDays < 0)
            throw new InvalidBorrowPeriodException(device.DeviceId, borrowDays, device.MaxBorrowDays);

        if (_items.Count >= Borrower.GetMaximumDevices())
            throw new BorrowLimitExceededException(Borrower.BorrowerId, Borrower.GetMaximumDevices());

        if (device.Status == DeviceStatus.UnderMaintenance)
            throw new DeviceUnavailableException(device.DeviceId, device.Status);

        device.MarkAsBorrowed();
        BorrowItem item = new BorrowItem(device, BorrowedDate, BorrowedDate.AddDays(borrowDays), 1, 5);
        _items.Add(item);
    }

    public decimal ReturnDevice(string deviceId, DateTime returnedDate)
    {
        BorrowItem? item = _items.FirstOrDefault(i => i.Device.DeviceId == deviceId);
        if (item == null)
            throw new DeviceNotInSlipException(deviceId);

        item.Return(returnedDate);
        item.Device.MarkAsAvailable();
        return CalculateLateFee(item);
    }

    private static decimal CalculateLateFee(BorrowItem item)
    {
        if (!item.ReturnedDate.HasValue) return 0;
        int lateDays = (item.ReturnedDate.Value.Date - item.DueDate.Date).Days;
        return lateDays * 10000m;
    }

    public decimal CalculateTotalLateFee()
    {
        return _items.Sum(CalculateLateFee);
    }
}


namespace LabW06;

public enum DeviceStatus
{
    Available,
    Borrowed,
    UnderMaintenance
}

public abstract class Device
{
    public string DeviceId { get; }
    public string Name { get; }
    public int YearInUse { get; }
    public decimal PurchasePrice { get; }

    // TỐI ƯU: Chỉ lớp Device được đổi trạng thái.
    public DeviceStatus Status { get; private set; } = DeviceStatus.Available;

    protected Device(string deviceId, string name, int yearInUse, decimal purchasePrice)
    {
        if (string.IsNullOrWhiteSpace(deviceId))
            throw new ArgumentException("Mã thiết bị không được rỗng.", nameof(deviceId));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tên thiết bị không được rỗng.", nameof(name));
        if (yearInUse < 1 || yearInUse > DateTime.Today.Year)
            throw new ArgumentOutOfRangeException(nameof(yearInUse), "Năm sử dụng không hợp lệ.");
        if (purchasePrice <= 0)
            throw new ArgumentOutOfRangeException(nameof(purchasePrice), "Giá mua phải lớn hơn 0.");

        DeviceId = deviceId.Trim();
        Name = name.Trim();
        YearInUse = yearInUse;
        PurchasePrice = purchasePrice;
    }

    public abstract decimal CalculateAnnualMaintenanceCost();

    public void MarkAsBorrowed()
    {
        if (Status != DeviceStatus.Available)
            throw new DeviceUnavailableException(DeviceId, Status);

        Status = DeviceStatus.Borrowed;
    }

    public void MarkAsAvailable()
    {
        if (Status != DeviceStatus.Borrowed)
            throw new InvalidOperationException("Thiết bị không ở trạng thái đang mượn.");

        Status = DeviceStatus.Available;
    }

    public void MarkUnderMaintenance()
    {
        if (Status == DeviceStatus.Borrowed)
            throw new InvalidOperationException("Không thể bảo trì thiết bị đang được mượn.");

        Status = DeviceStatus.UnderMaintenance;
    }

    public void CompleteMaintenance()
    {
        if (Status != DeviceStatus.UnderMaintenance)
            throw new InvalidOperationException("Thiết bị không trong quá trình bảo trì.");

        Status = DeviceStatus.Available;
    }

    public override string ToString() => $"{DeviceId} - {Name} - {Status}";
}

public abstract class TeachingDevice : Device
{
    public int MaxBorrowDays { get; }
    public int MaxBorrowPeriods { get; }

    // TỐI ƯU: Gom công thức bảo trì vào lớp cha.
    // Mỗi lớp con chỉ cần xác định tỷ lệ riêng.
    protected abstract decimal MaintenanceRate { get; }

    protected TeachingDevice(string id, string name, int year, decimal price,
        int maxBorrowDays, int maxBorrowPeriods = 5)
        : base(id, name, year, price)
    {
        if (maxBorrowDays <= 0)
            throw new ArgumentOutOfRangeException(nameof(maxBorrowDays));
        if (maxBorrowPeriods <= 0)
            throw new ArgumentOutOfRangeException(nameof(maxBorrowPeriods));

        MaxBorrowDays = maxBorrowDays;
        MaxBorrowPeriods = maxBorrowPeriods;
    }

    public sealed override decimal CalculateAnnualMaintenanceCost()
        => PurchasePrice * MaintenanceRate;
}

public sealed class AirConditionerRemote : TeachingDevice
{
    protected override decimal MaintenanceRate => 0.05m;

    public AirConditionerRemote(string id, string name, int year, decimal price,
        int maxDays, int maxPeriods = 5)
        : base(id, name, year, price, maxDays, maxPeriods) { }
}

public sealed class Microphone : TeachingDevice
{
    protected override decimal MaintenanceRate => 0.03m;

    public Microphone(string id, string name, int year, decimal price,
        int maxDays, int maxPeriods = 5)
        : base(id, name, year, price, maxDays, maxPeriods) { }
}

public sealed class ProjectorRemote : TeachingDevice
{
    protected override decimal MaintenanceRate => 0.04m;

    public ProjectorRemote(string id, string name, int year, decimal price,
        int maxDays, int maxPeriods = 5)
        : base(id, name, year, price, maxDays, maxPeriods) { }
}

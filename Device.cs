
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
    public DeviceStatus Status { get; private set; }

    protected Device(string deviceId, string name, int yearInUse, decimal purchasePrice)
    {
        if (string.IsNullOrWhiteSpace(deviceId))
            throw new ArgumentException("Ma thiet bi khong duoc rong.", nameof(deviceId));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Ten thiet bi khong duoc rong.", nameof(name));
        if (yearInUse < 1 || yearInUse > DateTime.Now.Year)
            throw new ArgumentOutOfRangeException(nameof(yearInUse), "Nam su dung khong hop le.");
        if (purchasePrice <= 0)
            throw new ArgumentOutOfRangeException(nameof(purchasePrice), "Gia mua phai lon hon 0.");

        DeviceId = deviceId.Trim();
        Name = name.Trim();
        YearInUse = yearInUse;
        PurchasePrice = purchasePrice;
        Status = DeviceStatus.Available;
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
            throw new InvalidOperationException("Chi thiet bi dang duoc muon moi co the tra.");
        Status = DeviceStatus.Available;
    }

    public void MarkUnderMaintenance()
    {
        if (Status == DeviceStatus.Borrowed)
            throw new InvalidOperationException("Khong the bao tri thiet bi dang duoc muon.");
        Status = DeviceStatus.UnderMaintenance;
    }

    public void CompleteMaintenance()
    {
        if (Status != DeviceStatus.UnderMaintenance)
            throw new InvalidOperationException("Thiet bi khong o trang thai bao tri.");
        Status = DeviceStatus.Available;
    }

    public override string ToString() => $"{DeviceId} - {Name} - {Status}";
}

public abstract class TeachingDevice : Device
{
    public int MaxBorrowDays { get; }
    public int MaxBorrowPeriods { get; }

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
}

public class AirConditionerRemote : TeachingDevice
{
    public AirConditionerRemote(string id, string name, int year, decimal price,
        int maxDays, int maxPeriods = 5)
        : base(id, name, year, price, maxDays, maxPeriods) { }

    public override decimal CalculateAnnualMaintenanceCost() => PurchasePrice * 0.05m;
}

public class Microphone : TeachingDevice
{
    public Microphone(string id, string name, int year, decimal price,
        int maxDays, int maxPeriods = 5)
        : base(id, name, year, price, maxDays, maxPeriods) { }

    public override decimal CalculateAnnualMaintenanceCost() => PurchasePrice * 0.03m;
}

public class ProjectorRemote : TeachingDevice
{
    public ProjectorRemote(string id, string name, int year, decimal price,
        int maxDays, int maxPeriods = 5)
        : base(id, name, year, price, maxDays, maxPeriods) { }

    public override decimal CalculateAnnualMaintenanceCost() => PurchasePrice * 0.04m;
}

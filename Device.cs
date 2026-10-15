
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
    public string Name { get; set; }
    public int YearInUse { get; set; }
    public decimal PurchasePrice { get; set; }
    public DeviceStatus Status { get; protected set; }

    protected Device(string deviceId, string name, int yearInUse, decimal purchasePrice)
    {
        DeviceId = deviceId;
        Name = name;
        YearInUse = yearInUse;
        PurchasePrice = purchasePrice;
        Status = DeviceStatus.Available;
    }

    public abstract decimal CalculateAnnualMaintenanceCost();

    public void MarkAsBorrowed() => Status = DeviceStatus.Borrowed;
    public void MarkAsAvailable() => Status = DeviceStatus.Available;
    public void MarkUnderMaintenance() => Status = DeviceStatus.UnderMaintenance;

    public override string ToString() => $"{DeviceId} - {Name} - {Status}";
}

public abstract class TeachingDevice : Device
{
    public int MaxBorrowDays { get; }

    protected TeachingDevice(string deviceId, string name, int yearInUse,
        decimal purchasePrice, int maxBorrowDays)
        : base(deviceId, name, yearInUse, purchasePrice)
    {
        MaxBorrowDays = maxBorrowDays;
    }
}

public class AirConditionerRemote : TeachingDevice
{
    public AirConditionerRemote(string id, string name, int year, decimal price, int maxDays)
        : base(id, name, year, price, maxDays) { }

    public override decimal CalculateAnnualMaintenanceCost() => PurchasePrice * 0.05m;
}

public class Microphone : TeachingDevice
{
    public Microphone(string id, string name, int year, decimal price, int maxDays)
        : base(id, name, year, price, maxDays) { }

    public override decimal CalculateAnnualMaintenanceCost() => PurchasePrice * 0.03m;
}

public class ProjectorRemote : TeachingDevice
{
    public ProjectorRemote(string id, string name, int year, decimal price, int maxDays)
        : base(id, name, year, price, maxDays) { }

    public override decimal CalculateAnnualMaintenanceCost() => PurchasePrice * 0.04m;
}

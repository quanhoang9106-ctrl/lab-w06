
namespace LabW06;

public class ClassRoom
{
    public string RoomId { get; }
    public string RoomName { get; }
    public int Capacity { get; }

    private readonly List<Device> _devices = new();

    public ClassRoom(string roomId, string roomName, int capacity)
    {
        RoomId = roomId;
        RoomName = roomName;
        Capacity = capacity;
    }

    public void AddDevice(Device device)
    {
        _devices.Add(device);
    }

    public bool RemoveDevice(string deviceId)
    {
        Device? device = FindDevice(deviceId);
        if (device == null) return false;
        return _devices.Remove(device);
    }

    public Device? FindDevice(string deviceId)
    {
        return _devices.FirstOrDefault(device => device.DeviceId == deviceId);
    }

    public decimal CalculateAnnualMaintenanceCost()
    {
        decimal total = 0;
        foreach (Device device in _devices)
        {
            if (device is AirConditionerRemote)
                total += device.PurchasePrice * 0.05m;
            else if (device is Microphone)
                total += device.PurchasePrice * 0.03m;
            else if (device is ProjectorRemote)
                total += device.PurchasePrice * 0.04m;
        }
        return total;
    }

    public List<Device> GetDevicesRequiringMaintenance()
    {
        return _devices.Where(device => device.Status == DeviceStatus.UnderMaintenance).ToList();
    }
}

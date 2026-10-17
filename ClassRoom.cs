
namespace LabW06;

public class ClassRoom
{
    private readonly List<Device> _devices = new();

    public string RoomId { get; }
    public string RoomName { get; }
    public int Capacity { get; }

    public ClassRoom(string roomId, string roomName, int capacity)
    {
        if (string.IsNullOrWhiteSpace(roomId))
            throw new ArgumentException("Ma phong khong hop le.", nameof(roomId));
        if (string.IsNullOrWhiteSpace(roomName))
            throw new ArgumentException("Ten phong khong hop le.", nameof(roomName));
        if (capacity <= 0)
            throw new ArgumentOutOfRangeException(nameof(capacity));

        RoomId = roomId.Trim();
        RoomName = roomName.Trim();
        Capacity = capacity;
    }

    public void AddDevice(Device device)
    {
        ArgumentNullException.ThrowIfNull(device);
        if (FindDevice(device.DeviceId) != null)
            throw new DuplicateDeviceException(device.DeviceId);

        _devices.Add(device);
    }

    public bool RemoveDevice(string deviceId)
    {
        Device? device = FindDevice(deviceId);
        return device != null && _devices.Remove(device);
    }

    public Device? FindDevice(string deviceId)
    {
        if (string.IsNullOrWhiteSpace(deviceId))
            throw new ArgumentException("Ma thiet bi khong hop le.", nameof(deviceId));

        return _devices.FirstOrDefault(d =>
            string.Equals(d.DeviceId, deviceId.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    public decimal CalculateAnnualMaintenanceCost()
    {
        decimal total = 0;
        foreach (Device device in _devices)
            total += device.CalculateAnnualMaintenanceCost();
        return total;
    }

    public List<Device> GetDevicesRequiringMaintenance()
    {
        return _devices.Where(d =>
            d.Status == DeviceStatus.UnderMaintenance ||
            DateTime.Now.Year - d.YearInUse > 5).ToList();
    }
}

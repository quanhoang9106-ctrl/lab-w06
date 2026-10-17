
namespace LabW06;

public class ClassRoom
{
    // TỐI ƯU: Dictionary giúp tra cứu nhanh theo mã thiết bị.
    // Không phân biệt chữ hoa/chữ thường trong mã.
    private readonly Dictionary<string, Device> _devices =
        new(StringComparer.OrdinalIgnoreCase);

    public string RoomId { get; }
    public string RoomName { get; }
    public int Capacity { get; }

    public ClassRoom(string roomId, string roomName, int capacity)
    {
        if (string.IsNullOrWhiteSpace(roomId))
            throw new ArgumentException("Mã phòng không hợp lệ.", nameof(roomId));
        if (string.IsNullOrWhiteSpace(roomName))
            throw new ArgumentException("Tên phòng không hợp lệ.", nameof(roomName));
        if (capacity <= 0)
            throw new ArgumentOutOfRangeException(nameof(capacity));

        RoomId = roomId.Trim();
        RoomName = roomName.Trim();
        Capacity = capacity;
    }

    public void AddDevice(Device device)
    {
        ArgumentNullException.ThrowIfNull(device);

        // TỐI ƯU: TryAdd vừa kiểm tra trùng vừa thêm thiết bị.
        if (!_devices.TryAdd(device.DeviceId, device))
            throw new DuplicateDeviceException(device.DeviceId);
    }

    public bool RemoveDevice(string deviceId)
    {
        ValidateDeviceId(deviceId);
        return _devices.Remove(deviceId.Trim());
    }

    public Device? FindDevice(string deviceId)
    {
        ValidateDeviceId(deviceId);

        return _devices.TryGetValue(deviceId.Trim(), out Device? device)
            ? device
            : null;
    }

    // TỐI ƯU: Dùng đa hình thay cho if/switch kiểm tra từng lớp.
    public decimal CalculateAnnualMaintenanceCost()
        => _devices.Values.Sum(device => device.CalculateAnnualMaintenanceCost());

    public List<Device> GetDevicesRequiringMaintenance()
        => _devices.Values.Where(device =>
            device.Status == DeviceStatus.UnderMaintenance ||
            DateTime.Today.Year - device.YearInUse > 5).ToList();

    // TỐI ƯU: Gom kiểm tra mã thiết bị để tránh lặp code.
    private static void ValidateDeviceId(string deviceId)
    {
        if (string.IsNullOrWhiteSpace(deviceId))
            throw new ArgumentException("Mã thiết bị không hợp lệ.", nameof(deviceId));
    }
}

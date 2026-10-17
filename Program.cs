
using System.Globalization;

namespace LabW06;

internal static class Program
{
    // Dữ liệu được giữ trong bộ nhớ khi chương trình chạy.
    private static readonly Dictionary<string, ClassRoom> Rooms =
        new(StringComparer.OrdinalIgnoreCase);

    private static readonly Dictionary<string, TeachingDevice> Devices =
        new(StringComparer.OrdinalIgnoreCase);

    private static readonly Dictionary<string, string> DeviceRooms =
        new(StringComparer.OrdinalIgnoreCase);

    private static readonly Dictionary<string, IBorrower> Borrowers =
        new(StringComparer.OrdinalIgnoreCase);

    private static readonly Dictionary<string, BorrowingSlip> Slips =
        new(StringComparer.OrdinalIgnoreCase);

    private static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        while (true)
        {
            PrintMenu();
            Console.Write("Chọn chức năng: ");
            string? choice = Console.ReadLine()?.Trim();
            Console.WriteLine();

            if (choice == "0")
            {
                Console.WriteLine("Đã thoát chương trình.");
                return;
            }

            try
            {
                switch (choice)
                {
                    case "1": AddRoom(); break;
                    case "2": AddDevice(); break;
                    case "3": ShowDevices(); break;
                    case "4": AddBorrower(); break;
                    case "5": CreateSlip(); break;
                    case "6": BorrowDevice(); break;
                    case "7": ReturnDevice(); break;
                    case "8": ShowSlips(); break;
                    case "9": ShowMaintenanceCosts(); break;
                    case "10": ShowStatistics(); break;
                    case "11": UpdateMaintenanceStatus(); break;
                    case "12": RunExceptionTests(); break;
                    default:
                        Console.WriteLine("Lựa chọn không hợp lệ.");
                        break;
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi tại giao diện, không dừng toàn bộ chương trình.
                Console.WriteLine($"[{ex.GetType().Name}] {ex.Message}");
            }

            Console.WriteLine();
        }
    }

    private static void PrintMenu()
    {
        Console.WriteLine("========== QUẢN LÝ THIẾT BỊ - LAB W06 ==========");
        Console.WriteLine(" 1. Thêm phòng học");
        Console.WriteLine(" 2. Thêm thiết bị vào phòng");
        Console.WriteLine(" 3. Xem danh sách thiết bị");
        Console.WriteLine(" 4. Thêm giảng viên / trợ giảng");
        Console.WriteLine(" 5. Tạo phiếu mượn");
        Console.WriteLine(" 6. Mượn thiết bị");
        Console.WriteLine(" 7. Trả thiết bị");
        Console.WriteLine(" 8. Xem các phiếu mượn");
        Console.WriteLine(" 9. Chi phí và danh sách cần bảo trì");
        Console.WriteLine("10. Thống kê phiếu trả muộn / chưa trả");
        Console.WriteLine("11. Bắt đầu / hoàn tất bảo trì thiết bị");
        Console.WriteLine("12. Chạy kiểm thử ngoại lệ");
        Console.WriteLine(" 0. Thoát");
    }

    private static void AddRoom()
    {
        string id = ReadText("Mã phòng: ");
        string name = ReadText("Tên phòng: ");
        int capacity = ReadInt("Sức chứa (> 0): ");

        var room = new ClassRoom(id, name, capacity);

        if (!Rooms.TryAdd(room.RoomId, room))
            throw new InvalidOperationException("Mã phòng đã tồn tại.");

        Console.WriteLine($"Đã thêm phòng {room.RoomId}.");
    }

    private static void AddDevice()
    {
        ClassRoom room = FindRequired(Rooms, "phòng");

        Console.WriteLine(
            "Loại thiết bị: 1. Điều khiển điều hòa  2. Micro  3. Điều khiển máy chiếu");

        int type = ReadInt("Chọn loại (1-3): ", 1, 3);

        string id = ReadText("Mã thiết bị: ");

        if (Devices.ContainsKey(id))
            throw new DuplicateDeviceException(id);

        string name = ReadText("Tên thiết bị: ");
        int year = ReadInt("Năm đưa vào sử dụng: ", 1, DateTime.Today.Year);
        decimal price = ReadMoney("Giá mua (ví dụ 600000): ");
        int maxDays = ReadInt("Số ngày mượn tối đa: ");
        int maxPeriods = ReadInt("Số tiết mượn tối đa: ");

        TeachingDevice device = type switch
        {
            1 => new AirConditionerRemote(
                id, name, year, price, maxDays, maxPeriods),

            2 => new Microphone(
                id, name, year, price, maxDays, maxPeriods),

            _ => new ProjectorRemote(
                id, name, year, price, maxDays, maxPeriods)
        };

        room.AddDevice(device);
        Devices.Add(device.DeviceId, device);
        DeviceRooms.Add(device.DeviceId, room.RoomId);

        Console.WriteLine($"Đã thêm {device.DeviceId} vào phòng {room.RoomId}.");
    }

    private static void ShowDevices()
    {
        if (Devices.Count == 0)
        {
            Console.WriteLine("Chưa có thiết bị. Hãy tạo phòng và thêm thiết bị trước.");
            return;
        }

        foreach (TeachingDevice device in Devices.Values)
        {
            Console.WriteLine(
                $"{device.DeviceId} | {device.Name} | Phòng {DeviceRooms[device.DeviceId]} " +
                $"| {device.Status} | {device.PurchasePrice:N0} VND " +
                $"| tối đa {device.MaxBorrowDays} ngày, {device.MaxBorrowPeriods} tiết");
        }
    }

    private static void AddBorrower()
    {
        Console.WriteLine(
            "1. Giảng viên (tối đa 3 thiết bị)  2. Trợ giảng (tối đa 2 thiết bị)");

        int type = ReadInt("Chọn loại (1-2): ", 1, 2);
        string id = ReadText("Mã người mượn: ");
        string fullName = ReadText("Họ và tên: ");

        IBorrower borrower = type == 1
            ? new Lecturer(id, fullName)
            : new TeachingAssistant(id, fullName);

        if (!Borrowers.TryAdd(borrower.BorrowerId, borrower))
            throw new InvalidOperationException("Mã người mượn đã tồn tại.");

        Console.WriteLine($"Đã thêm {borrower.FullName}.");
    }

    private static void CreateSlip()
    {
        string id = ReadText("Mã phiếu: ");
        IBorrower borrower = FindRequired(Borrowers, "người mượn");
        ClassRoom room = FindRequired(Rooms, "phòng");
        DateTime date = ReadDate("Ngày mượn (dd/MM/yyyy): ");

        var slip = new BorrowingSlip(id, borrower, room.RoomId, date);

        if (!Slips.TryAdd(slip.SlipId, slip))
            throw new InvalidOperationException("Mã phiếu đã tồn tại.");

        Console.WriteLine($"Đã tạo phiếu {slip.SlipId} cho {borrower.FullName}.");
    }

    private static void BorrowDevice()
    {
        BorrowingSlip slip = FindRequired(Slips, "phiếu");
        TeachingDevice device = FindRequired(Devices, "thiết bị");

        if (!string.Equals(
            slip.TeachingRoom,
            DeviceRooms[device.DeviceId],
            StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "Thiết bị không thuộc phòng ghi trên phiếu.");
        }

        int days = ReadInt("Số ngày muốn mượn: ");
        int fromPeriod = ReadInt("Mượn từ tiết: ");
        int toPeriod = ReadInt("Đến tiết: ");

        slip.AddDevice(device, days, fromPeriod, toPeriod);

        BorrowItem item = slip.Items.Last();

        Console.WriteLine(
            $"Mượn thành công {device.DeviceId}; hạn trả: {item.DueDate:dd/MM/yyyy}.");
    }

    private static void ReturnDevice()
    {
        BorrowingSlip slip = FindRequired(Slips, "phiếu");
        string id = ReadText("Mã thiết bị cần trả: ");
        DateTime returnDate = ReadDate("Ngày trả (dd/MM/yyyy): ");

        decimal fee = slip.ReturnDevice(id, returnDate);

        Console.WriteLine($"Trả thành công. Phí trả muộn: {fee:N0} VND.");
    }

    private static void ShowSlips()
    {
        if (Slips.Count == 0)
        {
            Console.WriteLine("Chưa có phiếu mượn.");
            return;
        }

        foreach (BorrowingSlip slip in Slips.Values)
        {
            Console.WriteLine(
                $"Phiếu {slip.SlipId} | {slip.Borrower.FullName} " +
                $"| Phòng {slip.TeachingRoom} | Ngày {slip.BorrowedDate:dd/MM/yyyy}");

            if (slip.Items.Count == 0)
                Console.WriteLine("  (Chưa có thiết bị)");

            foreach (BorrowItem item in slip.Items)
            {
                string returned = item.IsReturned
                    ? $"Đã trả {item.ReturnedDate:dd/MM/yyyy}"
                    : "Chưa trả";

                Console.WriteLine(
                    $"  - {item.Device.DeviceId}: hạn {item.DueDate:dd/MM/yyyy}, " +
                    $"tiết {item.FromPeriod}-{item.ToPeriod}, {returned}");
            }

            Console.WriteLine($"  Tổng phí: {slip.CalculateTotalLateFee():N0} VND");
        }
    }

    private static void ShowMaintenanceCosts()
    {
        if (Rooms.Count == 0)
        {
            Console.WriteLine("Chưa có phòng học.");
            return;
        }

        foreach (ClassRoom room in Rooms.Values)
        {
            Console.WriteLine(
                $"Phòng {room.RoomId}: {room.CalculateAnnualMaintenanceCost():N0} VND/năm");

            var needingMaintenance = room.GetDevicesRequiringMaintenance();

            if (needingMaintenance.Count == 0)
                Console.WriteLine("  Không có thiết bị cần xem xét bảo trì.");

            foreach (Device device in needingMaintenance)
            {
                Console.WriteLine(
                    $"  Cần xem xét: {device.DeviceId} - {device.Name} ({device.Status})");
            }
        }
    }

    private static void ShowStatistics()
    {
        if (Slips.Count == 0)
        {
            Console.WriteLine("Chưa có phiếu để thống kê.");
            return;
        }

        decimal totalFees = 0;

        foreach (BorrowingSlip slip in Slips.Values)
        {
            var late = slip.Items.Where(item =>
                item.ReturnedDate is DateTime d &&
                d.Date > item.DueDate.Date).ToList();

            var pending = slip.Items.Where(item => !item.IsReturned).ToList();

            foreach (BorrowItem item in late)
            {
                Console.WriteLine(
                    $"Phiếu {slip.SlipId} trả muộn: {item.Device.DeviceId}, " +
                    $"hạn {item.DueDate:dd/MM/yyyy}, " +
                    $"trả {item.ReturnedDate:dd/MM/yyyy}");
            }

            foreach (BorrowItem item in pending)
            {
                string overdue = DateTime.Today > item.DueDate.Date
                    ? " (đã quá hạn)"
                    : "";

                Console.WriteLine(
                    $"Phiếu {slip.SlipId} chưa trả: {item.Device.DeviceId}{overdue}");
            }

            totalFees += slip.CalculateTotalLateFee();
        }

        Console.WriteLine(
            $"Tổng phí trả muộn đã ghi nhận: {totalFees:N0} VND");
    }

    private static void UpdateMaintenanceStatus()
    {
        TeachingDevice device = FindRequired(Devices, "thiết bị");

        Console.WriteLine("1. Chuyển sang bảo trì  2. Hoàn tất bảo trì");
        int choice = ReadInt("Chọn (1-2): ", 1, 2);

        if (choice == 1)
            device.MarkUnderMaintenance();
        else
            device.CompleteMaintenance();

        Console.WriteLine(
            $"Trạng thái mới của {device.DeviceId}: {device.Status}");
    }

    // Các hàm hỗ trợ nhập dữ liệu

    private static T FindRequired<T>(
        Dictionary<string, T> source, string label) where T : class
    {
        string id = ReadText($"Nhập mã {label}: ");

        if (!source.TryGetValue(id, out T? value))
            throw new KeyNotFoundException(
                $"Không tìm thấy {label} có mã {id}.");

        return value;
    }

    private static string ReadText(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? value = Console.ReadLine();

            if (value is null)
                throw new InvalidOperationException(
                    "Không đọc được dữ liệu từ bàn phím.");

            if (!string.IsNullOrWhiteSpace(value))
                return value.Trim();

            Console.WriteLine("Không được để trống.");
        }
    }

    private static int ReadInt(
        string prompt,
        int minimum = 1,
        int maximum = int.MaxValue)
    {
        while (true)
        {
            string text = ReadText(prompt);

            if (int.TryParse(text, out int value) &&
                value >= minimum &&
                value <= maximum)
            {
                return value;
            }

            Console.WriteLine(
                $"Hãy nhập số nguyên từ {minimum} đến {maximum}.");
        }
    }

    private static decimal ReadMoney(string prompt)
    {
        while (true)
        {
            string text = ReadText(prompt);

            if (decimal.TryParse(
                text,
                NumberStyles.None,
                CultureInfo.InvariantCulture,
                out decimal value) && value > 0)
            {
                return value;
            }

            Console.WriteLine(
                "Hãy nhập số tiền dương, không có dấu chấm hoặc dấu phẩy.");
        }
    }

    private static DateTime ReadDate(string prompt)
    {
        while (true)
        {
            string text = ReadText(prompt);

            if (DateTime.TryParseExact(
                text,
                "dd/MM/yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime date))
            {
                return date;
            }

            Console.WriteLine(
                "Ngày không hợp lệ; ví dụ đúng: 15/10/2026.");
        }
    }

    // Bộ kiểm thử riêng, không làm thay đổi dữ liệu nhập từ menu.

    private static void RunExceptionTests()
    {
        Console.WriteLine("=== KIỂM THỬ NGOẠI LỆ ===");

        DateTime date = new(2026, 10, 15);

        static Microphone NewMicro(string id)
            => new(id, "Micro", 2024, 500000m, 1);

        static BorrowingSlip NewSlip()
            => new(
                "TEST",
                new Lecturer("GVTEST", "Giảng viên thử"),
                "P201",
                new DateTime(2026, 10, 15));

        ExpectException<DeviceUnavailableException>(
            "Mượn điều khiển điều hòa đang bảo trì", () =>
            {
                var air = new AirConditionerRemote(
                    "E01", "Điều khiển điều hòa", 2024, 300000m, 1);

                air.MarkUnderMaintenance();
                NewSlip().AddDevice(air, 1);
            });

        var assistantSlip = new BorrowingSlip(
            "P02",
            new TeachingAssistant("TG01", "Trợ giảng A"),
            "P201",
            date);

        assistantSlip.AddDevice(NewMicro("E02"), 1);
        assistantSlip.AddDevice(NewMicro("E03"), 1);

        var third = NewMicro("E04");

        ExpectException<BorrowLimitExceededException>(
            "Trợ giảng mượn thiết bị thứ ba",
            () => assistantSlip.AddDevice(third, 1));

        Check(
            "Vượt giới hạn không thay đổi dữ liệu",
            assistantSlip.Items.Count == 2 &&
            third.Status == DeviceStatus.Available);

        var duplicateSlip = NewSlip();
        var mic = NewMicro("E05");

        duplicateSlip.AddDevice(mic, 1);

        ExpectException<DuplicateDeviceException>(
            "Thêm trùng thiết bị",
            () => duplicateSlip.AddDevice(mic, 1));

        Check(
            "Không thêm phần tử trùng",
            duplicateSlip.Items.Count == 1);

        ExpectException<InvalidBorrowPeriodException>(
            "Mượn quá số ngày tối đa",
            () => NewSlip().AddDevice(NewMicro("E06"), 3));

        ExpectException<InvalidBorrowPeriodException>(
            "Số ngày mượn bằng 0",
            () => NewSlip().AddDevice(NewMicro("E10"), 0));

        var foreignProjector = new ProjectorRemote(
            "MC99", "Điều khiển máy chiếu", 2023, 600000m, 1);

        ExpectException<DeviceNotInSlipException>(
            "Trả máy chiếu không thuộc phiếu",
            () => NewSlip().ReturnDevice(foreignProjector.DeviceId, date));

        var returnedSlip = NewSlip();

        returnedSlip.AddDevice(NewMicro("E07"), 1);
        returnedSlip.ReturnDevice("E07", date.AddDays(1));

        ExpectException<DeviceAlreadyReturnedException>(
            "Trả cùng một thiết bị hai lần",
            () => returnedSlip.ReturnDevice("E07", date.AddDays(2)));

        var invalidDateSlip = NewSlip();
        var invalidDateDevice = NewMicro("E08");

        invalidDateSlip.AddDevice(invalidDateDevice, 1);

        ExpectException<InvalidReturnDateException>(
            "Ngày trả trước ngày mượn",
            () => invalidDateSlip.ReturnDevice("E08", date.AddDays(-1)));

        Check(
            "Ngày trả sai không đổi trạng thái",
            invalidDateSlip.Items[0].ReturnedDate == null &&
            invalidDateDevice.Status == DeviceStatus.Borrowed);

        ExpectException<InvalidLessonPeriodException>(
            "Khoảng tiết không hợp lệ",
            () => NewSlip().AddDevice(NewMicro("E09"), 1, 5, 2));

        var room = new ClassRoom("P01", "Phòng 01", 30);

        room.AddDevice(NewMicro("MIC01"));

        ExpectException<DuplicateDeviceException>(
            "Trùng mã thiết bị trong phòng",
            () => room.AddDevice(NewMicro("mic01")));
    }

    private static void ExpectException<T>(
        string description, Action action) where T : Exception
    {
        try
        {
            action();
            Console.WriteLine(
                $"[FAIL] {description}: Không có ngoại lệ.");
        }
        catch (T)
        {
            Console.WriteLine($"[PASS] {description}");
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"[FAIL] {description}: Sai ngoại lệ {ex.GetType().Name}.");
        }
    }

    private static void Check(string description, bool ok)
    {
        Console.WriteLine(
            $"[{(ok ? "PASS" : "FAIL")}] {description}");
    }
}

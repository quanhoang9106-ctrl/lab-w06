
using LabW06;

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.WriteLine("=== LAB W06 - QUẢN LÝ THIẾT BỊ ===");

try
{
    RunSuccessfulScenario();
    Console.WriteLine("\n=== KIỂM THỬ NGOẠI LỆ ===");
    RunExceptionTests();
}
catch (Exception ex)
{
    Console.WriteLine($"[LỖI CHƯA XỬ LÝ] {ex.Message}");
}
finally
{
    Console.WriteLine("\nKết thúc chương trình.");
}

static void RunSuccessfulScenario()
{
    ClassRoom room = new("P201", "Phòng học 201", 60);
    Lecturer lecturer = new("GV01", "Nguyễn Văn A");
    ProjectorRemote projector = new("MC01", "Điều khiển máy chiếu", 2023, 600000m, 2);
    AirConditionerRemote air = new("DH01", "Điều khiển điều hòa", 2019, 350000m, 2);
    Microphone microphone = new("MIC01", "Micro không dây", 2024, 900000m, 1);

    room.AddDevice(projector);
    room.AddDevice(air);
    room.AddDevice(microphone);

    Console.WriteLine($"Phòng: {room.RoomName}");
    Console.WriteLine($"Chi phí bảo trì dự kiến: {room.CalculateAnnualMaintenanceCost():N0} VND");
    Console.WriteLine($"Thiết bị cần xem xét bảo trì: {room.GetDevicesRequiringMaintenance().Count}");

    BorrowingSlip slip = new("PM01", lecturer, room.RoomId, new DateTime(2026, 10, 15));

    slip.AddDevice(projector, 1);
    slip.AddDevice(microphone, 1);
    slip.AddDevice(air, 2);

    Console.WriteLine($"Phiếu {slip.SlipId}: {slip.Items.Count} thiết bị");
    Console.WriteLine($"Phí máy chiếu: {slip.ReturnDevice("MC01", new DateTime(2026, 10, 16)):N0} VND");
    Console.WriteLine($"Phí micro: {slip.ReturnDevice("MIC01", new DateTime(2026, 10, 18)):N0} VND");
    Console.WriteLine($"Tổng phí trả muộn: {slip.CalculateTotalLateFee():N0} VND");

    // TỐI ƯU: Dùng LINQ để lọc đúng nhóm thiết bị cần hiển thị.
    foreach (BorrowItem item in slip.Items.Where(item =>
                 item.ReturnedDate is DateTime date && date > item.DueDate))
        Console.WriteLine($"Trả muộn: {item.Device.DeviceId}");

    foreach (BorrowItem item in slip.Items.Where(item => !item.IsReturned))
        Console.WriteLine($"Chưa trả: {item.Device.DeviceId}");
}

static void RunExceptionTests()
{
    DateTime borrowDate = new(2026, 10, 15);

    ExpectException<DeviceUnavailableException>("Mượn thiết bị đang bảo trì", () =>
    {
        var device = NewMicro("E01");
        device.MarkUnderMaintenance();
        NewSlip().AddDevice(device, 1);
    });

    var assistantSlip = new BorrowingSlip("P02",
        new TeachingAssistant("TG01", "Trợ giảng A"), "P201", borrowDate);

    assistantSlip.AddDevice(NewMicro("E02"), 1);
    assistantSlip.AddDevice(NewMicro("E03"), 1);
    Microphone third = NewMicro("E04");

    ExpectException<BorrowLimitExceededException>("Trợ giảng mượn thiết bị thứ ba",
        () => assistantSlip.AddDevice(third, 1));

    // TỐI ƯU: Kiểm tra cả trạng thái sau khi thao tác thất bại.
    Check("Phiếu và thiết bị giữ nguyên khi vượt hạn mức",
        assistantSlip.Items.Count == 2 && third.Status == DeviceStatus.Available);

    var duplicateSlip = NewSlip();
    var duplicate = NewMicro("E05");

    duplicateSlip.AddDevice(duplicate, 1);

    ExpectException<DuplicateDeviceException>("Thêm trùng thiết bị",
        () => duplicateSlip.AddDevice(duplicate, 1));

    Check("Không thêm phần tử trùng", duplicateSlip.Items.Count == 1);

    ExpectException<InvalidBorrowPeriodException>("Mượn micro quá một ngày",
        () => NewSlip().AddDevice(NewMicro("E06"), 3));

    ExpectException<InvalidBorrowPeriodException>("Số ngày mượn bằng 0",
        () => NewSlip().AddDevice(NewMicro("E10"), 0));

    ExpectException<DeviceNotInSlipException>("Trả thiết bị không có trong phiếu",
        () => NewSlip().ReturnDevice("KHONG_CO", borrowDate));

    var returnedSlip = NewSlip();
    var returnedDevice = NewMicro("E07");

    returnedSlip.AddDevice(returnedDevice, 1);
    returnedSlip.ReturnDevice("E07", borrowDate.AddDays(1));

    ExpectException<DeviceAlreadyReturnedException>("Trả hai lần",
        () => returnedSlip.ReturnDevice("E07", borrowDate.AddDays(2)));

    var invalidDateSlip = NewSlip();
    var invalidDateDevice = NewMicro("E08");

    invalidDateSlip.AddDevice(invalidDateDevice, 1);

    ExpectException<InvalidReturnDateException>("Ngày trả trước ngày mượn",
        () => invalidDateSlip.ReturnDevice("E08", borrowDate.AddDays(-1)));

    Check("Ngày trả sai không đổi trạng thái",
        invalidDateSlip.Items[0].ReturnedDate == null &&
        invalidDateDevice.Status == DeviceStatus.Borrowed);

    ExpectException<InvalidLessonPeriodException>("Khoảng tiết không hợp lệ",
        () => NewSlip().AddDevice(NewMicro("E09"), 1, 5, 2));

    var room = new ClassRoom("P01", "Phòng 01", 30);
    room.AddDevice(NewMicro("MIC01"));

    ExpectException<DuplicateDeviceException>("Trùng mã phòng học (khác chữ hoa/thường)",
        () => room.AddDevice(NewMicro("mic01")));
}

static Microphone NewMicro(string id)
    => new(id, "Micro", 2024, 500000m, 1);

static BorrowingSlip NewSlip()
    => new("TEST", new Lecturer("GVTEST", "Giảng viên thử"),
        "P201", new DateTime(2026, 10, 15));

static void ExpectException<T>(string name, Action action) where T : Exception
{
    try
    {
        action();
        Console.WriteLine($"[FAIL] {name}: không có ngoại lệ.");
    }
    catch (T ex)
    {
        Console.WriteLine($"[PASS] {name}: {ex.Message}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[FAIL] {name}: sai loại ngoại lệ {ex.GetType().Name}.");
    }
}

static void Check(string name, bool condition)
    => Console.WriteLine($"[{(condition ? "PASS" : "FAIL")}] {name}");

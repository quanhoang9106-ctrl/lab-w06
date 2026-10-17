
using LabW06;

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.WriteLine("=== LAB W06 - QUAN LY THIET BI ===");

try
{
    RunSuccessfulScenario();
    Console.WriteLine("\n=== KIEM THU NGOAI LE ===");
    RunExceptionTests();
}
catch (BorrowingException ex)
{
    Console.WriteLine($"Loi nghiep vu: {ex.Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"Loi khac: {ex.Message}");
}
finally
{
    Console.WriteLine("\nKet thuc chuong trinh.");
}

static void RunSuccessfulScenario()
{
    ClassRoom room = new("P201", "Phong hoc 201", 60);
    Lecturer lecturer = new("GV01", "Nguyen Van A");
    ProjectorRemote projector = new("MC01", "Dieu khien may chieu", 2023, 600000m, 2);
    AirConditionerRemote air = new("DH01", "Dieu khien dieu hoa", 2019, 350000m, 2);
    Microphone microphone = new("MIC01", "Micro khong day", 2024, 900000m, 1);

    room.AddDevice(projector);
    room.AddDevice(air);
    room.AddDevice(microphone);

    Console.WriteLine($"Phong: {room.RoomName}");
    Console.WriteLine($"Chi phi bao tri du kien: {room.CalculateAnnualMaintenanceCost():N0} VND");
    Console.WriteLine($"So thiet bi can xem xet bao tri: {room.GetDevicesRequiringMaintenance().Count}");

    BorrowingSlip slip = new("PM01", lecturer, room.RoomId, new DateTime(2026, 10, 15));
    slip.AddDevice(projector, 1);
    slip.AddDevice(microphone, 1);
    slip.AddDevice(air, 2);

    Console.WriteLine($"Phieu {slip.SlipId} muon {slip.Items.Count} thiet bi.");
    Console.WriteLine($"Phi tra may chieu: {slip.ReturnDevice("MC01", new DateTime(2026, 10, 16)):N0} VND");
    Console.WriteLine($"Phi tra micro: {slip.ReturnDevice("MIC01", new DateTime(2026, 10, 18)):N0} VND");
    Console.WriteLine($"Tong phi tra muon: {slip.CalculateTotalLateFee():N0} VND");

    foreach (BorrowItem item in slip.Items.Where(i => i.IsReturned && i.ReturnedDate!.Value.Date > i.DueDate.Date))
        Console.WriteLine($"Tra muon: {item.Device.DeviceId}");
    foreach (BorrowItem item in slip.Items.Where(i => !i.IsReturned))
        Console.WriteLine($"Chua tra: {item.Device.DeviceId}");
}

static void RunExceptionTests()
{
    DateTime borrowDate = new(2026, 10, 15);

    ExpectException<DeviceUnavailableException>("Muon thiet bi dang bao tri", () =>
    {
        var device = NewMicro("E01");
        device.MarkUnderMaintenance();
        NewSlip().AddDevice(device, 1);
    });

    ExpectException<BorrowLimitExceededException>("Tro giang muon thiet bi thu ba", () =>
    {
        var slip = new BorrowingSlip("P02", new TeachingAssistant("TG01", "Tro giang A"), "P201", borrowDate);
        slip.AddDevice(NewMicro("E02"), 1);
        slip.AddDevice(NewMicro("E03"), 1);
        var third = NewMicro("E04");
        try { slip.AddDevice(third, 1); }
        finally { Console.WriteLine($"  Kiem tra: {slip.Items.Count} item, E04 = {third.Status}"); }
    });

    ExpectException<DuplicateDeviceException>("Them trung mot thiet bi", () =>
    {
        var slip = NewSlip();
        var device = NewMicro("E05");
        slip.AddDevice(device, 1);
        slip.AddDevice(device, 1);
    });

    ExpectException<InvalidBorrowPeriodException>("Muon micro qua 1 ngay", () =>
        NewSlip().AddDevice(NewMicro("E06"), 3));

    ExpectException<DeviceNotInSlipException>("Tra thiet bi khong thuoc phieu", () =>
        NewSlip().ReturnDevice("KHONG_CO", borrowDate));

    ExpectException<DeviceAlreadyReturnedException>("Tra mot thiet bi hai lan", () =>
    {
        var slip = NewSlip();
        slip.AddDevice(NewMicro("E07"), 1);
        slip.ReturnDevice("E07", borrowDate.AddDays(1));
        slip.ReturnDevice("E07", borrowDate.AddDays(2));
    });

    ExpectException<InvalidReturnDateException>("Tra truoc ngay muon", () =>
    {
        var slip = NewSlip();
        slip.AddDevice(NewMicro("E08"), 1);
        slip.ReturnDevice("E08", borrowDate.AddDays(-1));
    });

    ExpectException<InvalidLessonPeriodException>("Khoang tiet khong hop le", () =>
        NewSlip().AddDevice(NewMicro("E09"), 1, 5, 2));
}

static Microphone NewMicro(string id) => new(id, "Micro", 2024, 500000m, 1);

static BorrowingSlip NewSlip() =>
    new("TEST", new Lecturer("GVTEST", "Giang vien thu"), "P201", new DateTime(2026, 10, 15));

static void ExpectException<T>(string name, Action action) where T : Exception
{
    try
    {
        action();
        Console.WriteLine($"[FAIL] {name}: khong phat sinh ngoai le.");
    }
    catch (T ex)
    {
        Console.WriteLine($"[PASS] {name}: {ex.Message}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[FAIL] {name}: nhan sai ngoai le {ex.GetType().Name}.");
    }
}

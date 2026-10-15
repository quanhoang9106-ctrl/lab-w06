
using LabW06;

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.WriteLine("=== LAB W06 - QUAN LY THIET BI ===");

try
{
    ClassRoom room = new ClassRoom("P201", "Phong hoc 201", 60);
    Lecturer lecturer = new Lecturer("GV01", "Nguyen Van A");

    ProjectorRemote projector = new ProjectorRemote("MC01", "Dieu khien may chieu", 2023, 600000m, 2);
    AirConditionerRemote airConditioner = new AirConditionerRemote("DH01", "Dieu khien dieu hoa", 2022, 350000m, 2);
    Microphone microphone = new Microphone("MIC01", "Micro khong day", 2024, 900000m, 1);

    room.AddDevice(projector);
    room.AddDevice(airConditioner);
    room.AddDevice(microphone);

    Console.WriteLine($"Phong: {room.RoomName}");
    Console.WriteLine($"Chi phi bao tri du kien: {room.CalculateAnnualMaintenanceCost():N0} VND");

    BorrowingSlip slip = new BorrowingSlip("PM01", lecturer, room.RoomId, new DateTime(2026, 10, 15));
    slip.AddDevice(projector, 1);
    slip.AddDevice(microphone, 1);

    Console.WriteLine($"Phieu {slip.SlipId}: {slip.Items.Count} thiet bi duoc muon");
    Console.WriteLine($"Trang thai micro khi muon: {microphone.Status}");

    decimal fee1 = slip.ReturnDevice(projector.DeviceId, new DateTime(2026, 10, 16));
    decimal fee2 = slip.ReturnDevice(microphone.DeviceId, new DateTime(2026, 10, 18));

    Console.WriteLine($"Phi tra may chieu: {fee1:N0} VND");
    Console.WriteLine($"Phi tra micro: {fee2:N0} VND");
    Console.WriteLine($"Tong phi: {slip.CalculateTotalLateFee():N0} VND");
    Console.WriteLine($"Trang thai micro sau khi tra: {microphone.Status}");
}
catch (BorrowingException ex)
{
    Console.WriteLine($"Loi nghiep vu: {ex.Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"Loi: {ex.Message}");
}
finally
{
    Console.WriteLine("Ket thuc chuong trinh.");
}

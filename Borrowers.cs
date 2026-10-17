
namespace LabW06;

public interface IBorrower
{
    string BorrowerId { get; }
    string FullName { get; }
    int GetMaximumDevices();
}

// TỐI ƯU: Tạo lớp cha chứa phần dữ liệu và kiểm tra dùng chung.
// Giúp tránh lặp code giữa Lecturer và TeachingAssistant.
public abstract class BorrowerBase : IBorrower
{
    public string BorrowerId { get; }
    public string FullName { get; }

    protected BorrowerBase(string borrowerId, string fullName)
    {
        if (string.IsNullOrWhiteSpace(borrowerId))
            throw new ArgumentException("Mã người mượn không hợp lệ.", nameof(borrowerId));
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Họ tên không hợp lệ.", nameof(fullName));

        BorrowerId = borrowerId.Trim();
        FullName = fullName.Trim();
    }

    public abstract int GetMaximumDevices();
}

public sealed class Lecturer : BorrowerBase
{
    public Lecturer(string borrowerId, string fullName)
        : base(borrowerId, fullName) { }

    public override int GetMaximumDevices() => 3;
}

public sealed class TeachingAssistant : BorrowerBase
{
    public TeachingAssistant(string borrowerId, string fullName)
        : base(borrowerId, fullName) { }

    public override int GetMaximumDevices() => 2;
}

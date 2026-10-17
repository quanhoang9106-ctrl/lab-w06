
namespace LabW06;

public interface IBorrower
{
    string BorrowerId { get; }
    string FullName { get; }
    int GetMaximumDevices();
}

public class Lecturer : IBorrower
{
    public string BorrowerId { get; }
    public string FullName { get; }

    public Lecturer(string borrowerId, string fullName)
    {
        if (string.IsNullOrWhiteSpace(borrowerId))
            throw new ArgumentException("Ma nguoi muon khong hop le.", nameof(borrowerId));
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Ho ten khong hop le.", nameof(fullName));

        BorrowerId = borrowerId.Trim();
        FullName = fullName.Trim();
    }

    public int GetMaximumDevices() => 3;
}

public class TeachingAssistant : IBorrower
{
    public string BorrowerId { get; }
    public string FullName { get; }

    public TeachingAssistant(string borrowerId, string fullName)
    {
        if (string.IsNullOrWhiteSpace(borrowerId))
            throw new ArgumentException("Ma nguoi muon khong hop le.", nameof(borrowerId));
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Ho ten khong hop le.", nameof(fullName));

        BorrowerId = borrowerId.Trim();
        FullName = fullName.Trim();
    }

    public int GetMaximumDevices() => 2;
}

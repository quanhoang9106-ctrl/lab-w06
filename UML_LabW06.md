# UML Class Diagram — Lab W06

Sơ đồ lớp được thiết kế từ chương trình C# Lab W06, gồm hai phần: nghiệp vụ và ngoại lệ. `Program.cs` là lớp giao diện Console quản lý các đối tượng qua `Dictionary`; để sơ đồ dễ đọc, các thao tác nhập/xuất và menu không liệt kê trong biểu đồ nghiệp vụ.

## 1. Sơ đồ lớp nghiệp vụ

```mermaid
classDiagram
    direction TB

    class DeviceStatus {
        <<enumeration>>
        Available
        Borrowed
        UnderMaintenance
    }

    class Device {
        <<abstract>>
        +string DeviceId
        +string Name
        +int YearInUse
        +decimal PurchasePrice
        +DeviceStatus Status
        +CalculateAnnualMaintenanceCost() decimal
        +MarkAsBorrowed() void
        +MarkAsAvailable() void
        +MarkUnderMaintenance() void
        +CompleteMaintenance() void
    }

    class TeachingDevice {
        <<abstract>>
        +int MaxBorrowDays
        +int MaxBorrowPeriods
        #decimal MaintenanceRate
        +CalculateAnnualMaintenanceCost() decimal
    }

    class AirConditionerRemote {
        #decimal MaintenanceRate
    }
    class Microphone {
        #decimal MaintenanceRate
    }
    class ProjectorRemote {
        #decimal MaintenanceRate
    }

    class ClassRoom {
        +string RoomId
        +string RoomName
        +int Capacity
        -Dictionary devices
        +AddDevice(device) void
        +RemoveDevice(deviceId) bool
        +FindDevice(deviceId) Device
        +CalculateAnnualMaintenanceCost() decimal
        +GetDevicesRequiringMaintenance() List
    }

    class IBorrower {
        <<interface>>
        +string BorrowerId
        +string FullName
        +GetMaximumDevices() int
    }
    class BorrowerBase {
        <<abstract>>
        +string BorrowerId
        +string FullName
        +GetMaximumDevices() int
    }
    class Lecturer {
        +GetMaximumDevices() int
    }
    class TeachingAssistant {
        +GetMaximumDevices() int
    }

    class BorrowItem {
        +TeachingDevice Device
        +DateTime BorrowedDate
        +DateTime DueDate
        +int FromPeriod
        +int ToPeriod
        +DateTime ReturnedDate
        +bool IsReturned
        +Return(returnedDate) void
    }
    class BorrowingSlip {
        +string SlipId
        +IBorrower Borrower
        +string TeachingRoom
        +DateTime BorrowedDate
        +IReadOnlyList Items
        +AddDevice(device,days) void
        +AddDevice(device,days,from,to) void
        +ReturnDevice(deviceId,date) decimal
        +CalculateTotalLateFee() decimal
    }

    Device <|-- TeachingDevice
    TeachingDevice <|-- AirConditionerRemote
    TeachingDevice <|-- Microphone
    TeachingDevice <|-- ProjectorRemote
    Device ..> DeviceStatus : uses

    IBorrower <|.. BorrowerBase
    BorrowerBase <|-- Lecturer
    BorrowerBase <|-- TeachingAssistant

    ClassRoom "1" o-- "0..*" Device : manages
    BorrowingSlip "1" *-- "0..*" BorrowItem : contains
    BorrowItem "0..*" --> "1" TeachingDevice : references
    BorrowingSlip "0..*" --> "1" IBorrower : belongs to
```

## 2. Hệ phân cấp ngoại lệ

```mermaid
classDiagram
    direction LR

    class Exception
    class BorrowingException
    class DeviceUnavailableException {
        +string DeviceId
        +DeviceStatus CurrentStatus
    }
    class BorrowLimitExceededException {
        +string BorrowerId
        +int MaximumDevices
    }
    class InvalidBorrowPeriodException {
        +string DeviceId
        +int RequestedDays
        +int MaximumDays
    }
    class DuplicateDeviceException {
        +string DeviceId
    }
    class DeviceNotInSlipException {
        +string DeviceId
    }
    class DeviceAlreadyReturnedException {
        +string DeviceId
    }
    class InvalidReturnDateException {
        +DateTime BorrowedDate
        +DateTime ReturnedDate
    }
    class InvalidLessonPeriodException {
        +int FromPeriod
        +int ToPeriod
        +int MaximumPeriods
    }

    Exception <|-- BorrowingException
    BorrowingException <|-- DeviceUnavailableException
    BorrowingException <|-- BorrowLimitExceededException
    BorrowingException <|-- InvalidBorrowPeriodException
    BorrowingException <|-- DuplicateDeviceException
    BorrowingException <|-- DeviceNotInSlipException
    BorrowingException <|-- DeviceAlreadyReturnedException
    BorrowingException <|-- InvalidReturnDateException
    BorrowingException <|-- InvalidLessonPeriodException
```

## 3. Ký hiệu UML

- `--|>`: kế thừa lớp.
- `..|>`: thực thi interface.
- `o--`: kết tập. Phòng học chứa tham chiếu đến các thiết bị, không sở hữu vòng đời của chúng.
- `*--`: hợp thành. Phiếu mượn chứa các BorrowItem.
- `-->`: liên kết/tham chiếu giữa hai lớp.
- `..>`: phụ thuộc (ví dụ Device dùng enum DeviceStatus).

**Chú ý:** Trong code hiện tại, `BorrowingSlip.TeachingRoom` chỉ lưu **mã phòng dạng `string`**, chưa phải tham chiếu tới đối tượng `ClassRoom`. Không nên vẽ quan hệ trực tiếp tới `ClassRoom` như thể đã có thuộc tính đó.

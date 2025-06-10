namespace BusinessObjects.Enums
{
    public enum BookingStatus
    {
        Pending = 0,      // Đang chờ xác nhận
        Confirmed = 1,    // Đã xác nhận
        Cancelled = 2,    // Đã hủy
        Completed = 3     // Đã hoàn thành (sau khi checkout)
    }
}

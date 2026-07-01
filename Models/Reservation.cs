namespace LibraryManagementSystem.Models;

public enum ReservationStatus
{
    Reserved,
    AvailableToCollect,
    Fulfilled,
    Cancelled,
    Expired
}

public class Reservation
{
    public int ReservationId { get; set; }
    public int BookId { get; set; }
    public int MemberId { get; set; }
    public DateTime ReserveDate { get; set; }
    public DateTime ReserveExpiry { get; set; }
    public ReservationStatus Status { get; private set; } = ReservationStatus.Reserved;

    public bool IsActive =>
        Status == ReservationStatus.Reserved ||
        Status == ReservationStatus.AvailableToCollect;

    public bool IsExpired() =>
        IsActive && DateTime.Now.Date > ReserveExpiry.Date;

    public void MarkAvailableToCollect() => Status = ReservationStatus.AvailableToCollect;
    public void MarkFulfilled() => Status = ReservationStatus.Fulfilled;
    public void Cancel() => Status = ReservationStatus.Cancelled;
    public void Expire() => Status = ReservationStatus.Expired;
}
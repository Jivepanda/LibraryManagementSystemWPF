namespace LibraryManagementSystem.Models;

public class Reservation
{
    public int ReservationId { get; set; }
    public int BookId { get; set; }
    public int MemberId { get; set; }
    public DateTime ReserveDate { get; set; }
    public DateTime ReserveExpiry { get; set; }
    public string Status { get; set; } = "Active";

    public void MarkFulfilled()
    {
        Status = "Fulfilled";
    }

    public void Cancel()
    {
        Status = "Cancelled";
    }
}

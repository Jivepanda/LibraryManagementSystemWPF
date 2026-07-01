namespace LibraryManagementSystem.ViewModels
{
    public class BorrowedBookViewModel
    {
        public int BookId { get; set; }
        public int? LoanId { get; set; }
        public int? ReservationId { get; set; }

        public string Title { get; set; } = string.Empty;
        public string DateString { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;

        public bool IsReservationReadyToCollect => Status == "Available to Collect";
        public bool IsActiveLoan => Status == "Active" || Status == "Overdue";
    }
}
namespace LibraryManagementSystem.Models;

public class Loan
{
    public int LoanId { get; set; }
    public int BookId { get; set; }
    public int MemberId { get; set; }
    public DateTime BorrowDate { get; set; }
    public DateTime DueDate { get; set; }
    public bool Returned { get; set; }
    public DateTime? ReturnDate { get; set; }

    public void MarkReturned()
    {
        Returned = true;
        ReturnDate = DateTime.Now.Date;
    }

    public bool IsOverdue()
    {
        return !Returned && DateTime.Now.Date > DueDate.Date;
    }
}
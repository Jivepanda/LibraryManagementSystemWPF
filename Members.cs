namespace LibraryManagementSystem.Models;

public class Member : User
{
    public int MaxLoans { get; set; } = 3;

    public bool CanBorrowMore(List<Loan> loans)
    {
        return loans.Count(l => l.MemberId == UserId && !l.Returned) < MaxLoans;
    }
}

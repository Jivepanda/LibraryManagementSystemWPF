using LibraryManagementSystem.Models;

public class Member
{
    public int MemberId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Role { get; set; } = "Member";
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public int MaxLoans { get; set; } = 3;

    public bool CanBorrowMore(List<Loan> loans)
    {
        return loans.Count(l => l.MemberId == MemberId && !l.Returned) < MaxLoans;
    }
}
namespace LibraryManagementSystem.Models;

public class Book
{
    public int BookId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public DateTime PublicationDate { get; set; }
    public int TotalCopies { get; set; }
    public int AvailableCopies { get; set; }

    public bool IsAvailable()
    {
        return AvailableCopies > 0;
    }

    public bool BorrowCopy()
    {
        if (!IsAvailable()) return false;
        AvailableCopies--;
        return true;
    }

    public bool ReturnCopy()
    {
        if (AvailableCopies >= TotalCopies) return false;
        AvailableCopies++;
        return true;
    }
}

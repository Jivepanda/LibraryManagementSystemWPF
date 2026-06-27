using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Services;

public class LibrarySystem
{
    private readonly FileStorageService _storageService;

    public List<Book> Books { get; set; }
    public List<Member> Members { get; set; }
    private readonly string membersFile = "Data/members.json";
    public List<Loan> Loans { get; set; }
    public List<Reservation> Reservations { get; set; }

    private readonly string booksFile = "Data/books.json";
    private readonly string usersFile = "Data/users.json";
    private readonly string loansFile = "Data/loans.json";
    private readonly string reservationsFile = "Data/reservations.json";

    public LibrarySystem()
    {
        _storageService = new FileStorageService();
        Books = _storageService.LoadData<Book>(booksFile);
        Members = _storageService.LoadData<Member>(membersFile);
        Loans = _storageService.LoadData<Loan>(loansFile);
        Reservations = _storageService.LoadData<Reservation>(reservationsFile);
    }

    public void SaveAllData()
    {
        _storageService.SaveData(booksFile, Books);
        _storageService.SaveData(membersFile, Members);
        _storageService.SaveData(loansFile, Loans);
        _storageService.SaveData(reservationsFile, Reservations);
    }

    public List<Book> SearchBooks(string query)
    {
        return Books.Where(b =>
            b.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
            b.Author.Contains(query, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public Book? ViewBookDetails(int bookId)
    {
        return Books.FirstOrDefault(b => b.BookId == bookId);
    }

    public void ListAllBooks()
    {
        if (Books.Count == 0)
        {
            Console.WriteLine("No books available in the library.");
            return;
        }

        Console.WriteLine("\n==== All Books ====\n");

        foreach (var book in Books)
        {
            Console.WriteLine($"Book ID: {book.BookId}");
            Console.WriteLine($"Title: {book.Title}");
            Console.WriteLine($"Author: {book.Author}");
            Console.WriteLine($"Publication Date: {book.PublicationDate:dd/MM/yyyy}");
            Console.WriteLine($"Total Copies: {book.TotalCopies}");
            Console.WriteLine($"Available Copies: {book.AvailableCopies}");
            Console.WriteLine(new string('-', 40));
        }
    }

    public string BorrowBook(int memberId, int bookId)
    {
        var book = Books.FirstOrDefault(b => b.BookId == bookId);
        var member = Members.FirstOrDefault(m => m.UserId == memberId);

        if (book == null)
            return "Book not found.";

        if (member == null)
            return "Member not found.";

        if (!member.CanBorrowMore(Loans))
            return "Member cannot borrow more than three books at a time.";

        if (!book.IsAvailable())
            return "Book is not available.";

        bool hasOverdueLoan = Loans.Any(l =>
            l.MemberId == memberId &&
            !l.Returned &&
            l.IsOverdue());

        if (hasOverdueLoan)
            return "Member must return overdue books before borrowing another one.";

        bool borrowed = book.BorrowCopy();

        if (!borrowed)
            return "Borrowing failed.";

        int nextLoanId = Loans.Count == 0 ? 1 : Loans.Max(l => l.LoanId) + 1;

        Loan newLoan = new Loan
        {
            LoanId = nextLoanId,
            BookId = bookId,
            MemberId = memberId,
            BorrowDate = DateTime.Now.Date,
            DueDate = DateTime.Now.Date.AddDays(14),
            Returned = false
        };

        Loans.Add(newLoan);
        SaveAllData();

        return $"Book borrowed successfully. Due date: {newLoan.DueDate:dd/MM/yyyy}";
    }

    public string ReturnBook(int memberId, int bookId)
    {
        var book = Books.FirstOrDefault(b => b.BookId == bookId);
        var loan = Loans.FirstOrDefault(l =>
            l.BookId == bookId &&
            l.MemberId == memberId &&
            !l.Returned);

        if (book == null)
            return "Book not found.";

        if (loan == null)
            return "No active loan found for this member and book.";

        bool wasOverdue = loan.IsOverdue();

        bool returned = book.ReturnCopy();

        if (!returned)
            return "Return failed. Available copies would exceed total copies.";

        loan.MarkReturned();
        SaveAllData();

        if (wasOverdue)
            return "Book returned successfully. This book was returned late and may incur late fees.";

        return "Book returned successfully.";
    }

    // FIX 1: Changed `group.Key` to `memberId`, and renamed `user` to `member`
    public void ViewBorrowedBooksByMember(int memberId)
    {
        var member = Members.FirstOrDefault(m => m.UserId == memberId);

        if (member == null)
        {
            Console.WriteLine("Member not found.");
            return;
        }

        var memberLoans = Loans
            .Where(l => l.MemberId == memberId && !l.Returned)
            .ToList();

        if (memberLoans.Count == 0)
        {
            Console.WriteLine("This member has no borrowed books.");
            return;
        }

        Console.WriteLine($"\n==== Borrowed Books for {member.Name} ====\n");

        foreach (var loan in memberLoans)
        {
            var book = Books.FirstOrDefault(b => b.BookId == loan.BookId);

            Console.WriteLine($"Loan ID: {loan.LoanId}");
            Console.WriteLine($"Book Title: {book?.Title ?? "Unknown Book"}");
            Console.WriteLine($"Author: {book?.Author ?? "Unknown Author"}");
            Console.WriteLine($"Borrow Date: {loan.BorrowDate:dd/MM/yyyy}");
            Console.WriteLine($"Due Date: {loan.DueDate:dd/MM/yyyy}");
            Console.WriteLine($"Overdue: {(loan.IsOverdue() ? "Yes" : "No")}");
            Console.WriteLine(new string('-', 40));
        }
    }

    // FIX 2: Changed `Users` to `Members`
    public string ReserveBook(int memberId, int bookId)
    {
        var book = Books.FirstOrDefault(b => b.BookId == bookId);
        var member = Members.FirstOrDefault(m => m.UserId == memberId);

        if (book == null)
            return "Book not found.";

        if (member == null)
            return "Member not found.";

        bool alreadyReserved = Reservations.Any(r =>
            r.BookId == bookId &&
            r.MemberId == memberId &&
            r.Status == "Active");

        if (alreadyReserved)
            return "This member already has an active reservation for this book.";

        int nextReservationId = Reservations.Count == 0
            ? 1
            : Reservations.Max(r => r.ReservationId) + 1;

        Reservation newReservation = new Reservation
        {
            ReservationId = nextReservationId,
            BookId = bookId,
            MemberId = memberId,
            ReserveDate = DateTime.Now.Date,
            ReserveExpiry = DateTime.Now.Date.AddDays(7),
            Status = "Active"
        };

        Reservations.Add(newReservation);
        SaveAllData();

        return $"Book reserved successfully until {newReservation.ReserveExpiry:dd/MM/yyyy}.";
    }

    // FIX 3: Renamed `user` to `member`, and fixed closing brace so ListUsersWithBorrowedBooks is no longer nested inside
    public void ListAllLoans()
    {
        var activeLoans = Loans.Where(l => !l.Returned).ToList();

        if (activeLoans.Count == 0)
        {
            Console.WriteLine("No active loans found.");
            return;
        }

        Console.WriteLine("\n==== All Active Loans ====\n");

        foreach (var loan in activeLoans)
        {
            var book = Books.FirstOrDefault(b => b.BookId == loan.BookId);
            var member = Members.FirstOrDefault(m => m.UserId == loan.MemberId);

            Console.WriteLine($"Loan ID: {loan.LoanId}");
            Console.WriteLine($"Book Title: {book?.Title ?? "Unknown Book"}");
            Console.WriteLine($"Borrowed By: {member?.Name ?? "Unknown Member"}");
            Console.WriteLine($"Due Date: {loan.DueDate:dd/MM/yyyy}");
            Console.WriteLine($"Overdue: {(loan.IsOverdue() ? "Yes" : "No")}");
            Console.WriteLine(new string('-', 40));
        }
    }

    // FIX 4: Changed `Users` to `Members`
    public void ListUsersWithBorrowedBooks()
    {
        var activeLoans = Loans.Where(l => !l.Returned).ToList();

        if (activeLoans.Count == 0)
        {
            Console.WriteLine("No users currently have borrowed books.");
            return;
        }

        var groupedLoans = activeLoans.GroupBy(l => l.MemberId);

        Console.WriteLine("\n==== Users With Borrowed Books ====\n");
    }
}


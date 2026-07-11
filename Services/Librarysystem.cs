using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Services;
//
public class LibrarySystem
{
    private readonly FileStorageService _storageService;

    private readonly string _booksFile = "Data/books.json";
    private readonly string _membersFile = "Data/members.json";
    private readonly string _loansFile = "Data/loans.json";
    private readonly string _reservationsFile = "Data/reservations.json";

    public List<Book> Books { get; private set; }
    public List<Member> Members { get; private set; }
    public List<Loan> Loans { get; private set; }
    public List<Reservation> Reservations { get; private set; }

    public ReservationService ReservationsManager { get; }

    public LibrarySystem()
    {
        _storageService = new FileStorageService();
        Books = _storageService.LoadData<Book>(_booksFile);
        Members = _storageService.LoadData<Member>(_membersFile);
        Loans = _storageService.LoadData<Loan>(_loansFile);
        Reservations = _storageService.LoadData<Reservation>(_reservationsFile);

        // FIX 1: Loans was being passed in place of Reservations, args were shifted.
        // FIX 2: ReservationService now receives Loans so it can check active loan counts.
        ReservationsManager = new ReservationService(
            Books,
            Members,
            Loans,
            Reservations,
            SaveAllData);
    }

    public void SaveAllData()
    {
        _storageService.SaveData(_booksFile, Books);
        _storageService.SaveData(_membersFile, Members);
        _storageService.SaveData(_loansFile, Loans);
        _storageService.SaveData(_reservationsFile, Reservations);
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
        var member = Members.FirstOrDefault(m => m.MemberId == memberId);

        if (book == null) return "Book not found.";
        if (member == null) return "Member not found.";

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

        if (!book.BorrowCopy())
            return "Borrowing failed.";

        int nextLoanId = Loans.Count == 0 ? 1 : Loans.Max(l => l.LoanId) + 1;

        var newLoan = new Loan
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

    // This allows the method to find the loan in the Loans list and update it.
    public string ReturnLoan(int loanId)
    {
        var loan = Loans.FirstOrDefault(l => l.LoanId == loanId && !l.Returned);
        if (loan == null)
            return "Loan not found or already returned.";

        var book = Books.FirstOrDefault(b => b.BookId == loan.BookId);
        if (book == null)
            return "Book not found.";

        bool returnedToShelf = book.ReturnCopy();
        if (!returnedToShelf)
            return "Book return failed because available copies would exceed total copies.";
        loan.MarkReturned();


        var nextReservation = Reservations
            .Where(r => r.BookId == loan.BookId && r.Status == ReservationStatus.Reserved)
            .OrderBy(r => r.ReserveDate)
            .FirstOrDefault();

        if (nextReservation != null)
        {
            nextReservation.MarkAvailableToCollect();
            SaveAllData();
            return "Book returned successfully.";
        }

        SaveAllData();
        return "Book returned successfully.";
    }
    public void UpdateReadyToCollectReservationsForMember(int memberId)
    {
        var memberReservations = Reservations
            .Where(r => r.MemberId == memberId && r.Status == ReservationStatus.Reserved)
            .ToList();

        bool changed = false;

        foreach (var reservation in memberReservations)
        {
            var book = Books.FirstOrDefault(b => b.BookId == reservation.BookId);
            if (book == null || book.AvailableCopies <= 0)
                continue;

            bool alreadyCollectable = Reservations.Any(r =>
                r.BookId == reservation.BookId &&
                r.Status == ReservationStatus.AvailableToCollect);

            if (alreadyCollectable)
                continue;

            var nextInQueue = Reservations
                .Where(r => r.BookId == reservation.BookId && r.Status == ReservationStatus.Reserved)
                .OrderBy(r => r.ReserveDate)
                .FirstOrDefault();

            if (nextInQueue != null && nextInQueue.ReservationId == reservation.ReservationId)
            {
                reservation.MarkAvailableToCollect();
                changed = true;
            }
        }

        if (changed)
            SaveAllData();
    }
    public void RepairReservationAvailability(int bookId)
    {
        var book = Books.FirstOrDefault(b => b.BookId == bookId);
        if (book == null || book.AvailableCopies <= 0)
            return;

        var nextReservation = Reservations
            .Where(r => r.BookId == bookId && r.Status == ReservationStatus.Reserved)
            .OrderBy(r => r.ReserveDate)
            .FirstOrDefault();

        if (nextReservation != null)
        {
            nextReservation.MarkAvailableToCollect();
            SaveAllData();
        }
    }
    public void ViewBorrowedBooksByMember(int memberId)
    {
        var member = Members.FirstOrDefault(m => m.MemberId == memberId);
        if (member == null)
        {
            Console.WriteLine("Member not found.");
            return;
        }

        var memberLoans = Loans.Where(l => l.MemberId == memberId && !l.Returned).ToList();
        if (memberLoans.Count == 0)
        {
            Console.WriteLine("This member has no borrowed books.");
            return;
        }

        Console.WriteLine($"\n==== Borrowed Books for {member.FirstName} {member.LastName} ====\n");
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
            var member = Members.FirstOrDefault(m => m.MemberId == loan.MemberId);

            Console.WriteLine($"Loan ID: {loan.LoanId}");
            Console.WriteLine($"Book Title: {book?.Title ?? "Unknown Book"}");
            Console.WriteLine($"Borrowed By: {member?.FirstName} {member?.LastName ?? "Unknown Member"}");
            Console.WriteLine($"Due Date: {loan.DueDate:dd/MM/yyyy}");
            Console.WriteLine($"Overdue: {(loan.IsOverdue() ? "Yes" : "No")}");
            Console.WriteLine(new string('-', 40));
        }
    }

    // FIX 4: Completed the missing loop body.
    public void ListUsersWithBorrowedBooks()
    {
        var activeLoans = Loans.Where(l => !l.Returned).ToList();
        if (activeLoans.Count == 0)
        {
            Console.WriteLine("No users currently have borrowed books.");
            return;
        }

        Console.WriteLine("\n==== Users With Borrowed Books ====\n");
        foreach (var group in activeLoans.GroupBy(l => l.MemberId))
        {
            var member = Members.FirstOrDefault(m => m.MemberId == group.Key);
            Console.WriteLine($"Member: {member?.FirstName} {member?.LastName ?? "Unknown"} (ID: {group.Key})");
            Console.WriteLine($"Active Loans: {group.Count()}");
            Console.WriteLine(new string('-', 40));
        }
    }

    public class ReservationService
    {
        private const int MaxActiveReservations = 3;
        private const int ReservationDurationDays = 7;

        private readonly List<Book> _books;
        private readonly List<Member> _members;
        private readonly List<Loan> _loans;
        private readonly List<Reservation> _reservations;
        private readonly Action _saveAllData;

        // FIX 2:                                                                                                     Added List<Loan> so ReserveBook can check active loan counts.
        public ReservationService(
            List<Book> books,
            List<Member> members,
            List<Loan> loans,
            List<Reservation> reservations,
            Action saveAllData)
        {
            _books = books;
            _members = members;
            _loans = loans;
            _reservations = reservations;
            _saveAllData = saveAllData;
        }

        public string ReserveBook(int memberId, int bookId)
        {
            var book = _books.FirstOrDefault(b => b.BookId == bookId);
            if (book == null) return "Book not found.";

            var member = _members.FirstOrDefault(m => m.MemberId == memberId);
            if (member == null) return "Member not found.";

            if (GetActiveReservationCount(memberId) >= MaxActiveReservations)
                return "Member cannot have more than three active reservations.";

            bool alreadyReserved = _reservations.Any(r =>
                r.BookId == bookId &&
                r.MemberId == memberId &&
                r.IsActive);

            if (alreadyReserved)
                return "This member already has an active reservation for this book.";

            int activeLoans = _loans.Count(l => l.MemberId == memberId && !l.Returned);
            bool noCopiesAvailable = book.AvailableCopies <= 0;
            bool memberAtLoanLimit = activeLoans >= 3;

            if (!noCopiesAvailable && !memberAtLoanLimit)
                return "This book is available to borrow. Reservations are only allowed when no copies are available or the member already has three active loans.";

            int nextReservationId = _reservations.Count == 0
                ? 1
                : _reservations.Max(r => r.ReservationId) + 1;

            var newReservation = new Reservation
            {
                ReservationId = nextReservationId,
                BookId = bookId,
                MemberId = memberId,
                ReserveDate = DateTime.Now.Date,
                ReserveExpiry = DateTime.Now.Date.AddDays(ReservationDurationDays)
            };

            _reservations.Add(newReservation);
            _saveAllData();

            return $"Book reserved successfully until {newReservation.ReserveExpiry:dd/MM/yyyy}.";
        }

        public Reservation? GetNextReservedInQueue(int bookId)
        {
            return _reservations
                .Where(r => r.BookId == bookId && r.Status == ReservationStatus.Reserved)
                .OrderBy(r => r.ReserveDate)
                .FirstOrDefault();
        }

        public int GetActiveReservationCount(int memberId)
        {
            return _reservations.Count(r => r.MemberId == memberId && r.IsActive);
        }
    }

    public string CollectReservedBook(int memberId, int reservationId)
    {
        var reservation = Reservations.FirstOrDefault(r =>
            r.ReservationId == reservationId &&
            r.MemberId == memberId);

        if (reservation == null)
            return "Reservation not found.";

        RepairReservationAvailability(reservation.BookId);

        if (reservation.Status != ReservationStatus.AvailableToCollect)
            return "This reservation is not ready to collect.";

        var member = Members.FirstOrDefault(m => m.MemberId == memberId);
        if (member == null)
            return "Member not found.";

        var book = Books.FirstOrDefault(b => b.BookId == reservation.BookId);
        if (book == null)
            return "Book not found.";

        if (!member.CanBorrowMore(Loans))
            return "Member cannot borrow more than three books at a time.";

        bool hasOverdueLoan = Loans.Any(l =>
            l.MemberId == memberId &&
            !l.Returned &&
            l.IsOverdue());

        if (hasOverdueLoan)
            return "Member must return overdue books before collecting this book.";

        int nextLoanId = Loans.Count == 0 ? 1 : Loans.Max(l => l.LoanId) + 1;

        var newLoan = new Loan
        {
            LoanId = nextLoanId,
            BookId = reservation.BookId,
            MemberId = memberId,
            BorrowDate = DateTime.Now.Date,
            DueDate = DateTime.Now.Date.AddDays(14),
            Returned = false
        };

        Loans.Add(newLoan);
        reservation.MarkFulfilled();
        SaveAllData();

        return $"Reserved book collected successfully. Due date: {newLoan.DueDate:dd/MM/yyyy}";
    }
    public Member RegisterMember(string firstName, string lastName, string email, string phone)
    {
        return RegisterUser(firstName, lastName, email, phone, "Member");
    }
    public Member RegisterUser(string firstName, string lastName, string email, string phone, string role)
    {
        return RegisterUserInternal(firstName, lastName, email, phone, role);
    }
    private Member RegisterUserInternal(string firstName, string lastName, string email, string phone, string role)
    {
        firstName = firstName.Trim();
        lastName = lastName.Trim();
        email = email.Trim();
        phone = phone.Trim();
        role = role.Trim();

        if (string.IsNullOrWhiteSpace(firstName))
            throw new InvalidOperationException("First name is required.");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new InvalidOperationException("Last name is required.");

        if (string.IsNullOrWhiteSpace(email))
            throw new InvalidOperationException("Email is required.");

        if (string.IsNullOrWhiteSpace(phone))
            throw new InvalidOperationException("Phone is required.");

        if (Members.Any(m => m.Email.Equals(email, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("A member with this email already exists.");

        int nextMemberId = Members.Count == 0 ? 1 : Members.Max(m => m.MemberId) + 1;

        var newMember = new Member
        {
            MemberId = nextMemberId,
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Phone = phone,
            Role = role,
            IsActive = true
        };

        Members.Add(newMember);
        SaveAllData();

        return newMember;
    }
}

# The Plot Twist Library

A C# WPF library management system with separate member and staff flows for borrowing, returning, reserving, collecting reserved books, and viewing borrowing history.

## Features

- Member login using Member ID
- User dashboard for borrowing, searching, returning, reserving, and collecting books
- Borrowing history view with status tracking
- Reservation handling with ready-to-collect workflow
- Member registration from the login screen
- JSON-based data storage for books, members, loans, and reservations

## Built With

- C#
- WPF (Windows Presentation Foundation)
- JSON file storage
- Visual Studio

## Project Structure

```text
LibraryManagementSystem/
├── Models/
│   ├── Book.cs
│   ├── Loan.cs
│   ├── Members.cs
│   ├── Reservation.cs
│   ├── Staff.cs
│   └── User.cs
├── Services/
│   ├── FileStorageService.cs
│   └── LibrarySystem.cs
├── ViewModels/
│   └── BorrowedBookViewModel.cs
├── Windows/
│   ├── LoginWindow.xaml
│   ├── UserDashboardWindow.xaml
│   ├── SearchBooksWindow.xaml
│   ├── ReturnBookWindow.xaml
│   └── RegisterMemberWindow.xaml
└── Data/
    ├── books.json
    ├── members.json
    ├── loans.json
    └── reservations.json
```

## Current Functions

### Member Side

- Sign in with Member ID
- Register as a new member
- Search for books
- Borrow books
- Return books
- Reserve unavailable books
- Collect reserved books when available
- View current loans and reservation status
- View full borrowing history

### System Rules

- Members can borrow up to three books at a time
- Members cannot borrow if they have overdue books
- Borrowed books are given a due date of 14 days
- Reservations can remain active for up to 7 days

## How to Run

1. Open the solution in Visual Studio
2. Restore packages if prompted
3. Build the solution
4. Run the WPF application
5. Use an existing Member ID from `members.json`, or register a new member from the login screen

## Sample Login

Use any existing member record from the data file, for example:

- Member ID: `1`
- Name: Alice Martin

## Data Storage

The application stores data in JSON files:

- `books.json`
- `members.json`
- `loans.json`
- `reservations.json`

This makes the project simple to test and easy to demonstrate without a database setup.

## In Progress

- Staff dashboard improvements
- Staff registration flow with role selection
- Additional UI polish and validation
- Expanded reporting and management tools

## Notes

This project was built as a desktop application prototype for a library system and is suitable for coursework, demonstrations, and further extension into a larger management platform.****

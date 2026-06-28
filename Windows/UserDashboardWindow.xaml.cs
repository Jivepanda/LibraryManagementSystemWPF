using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services;
using LibraryManagementSystem.Helpers;
using LibraryManagementSystem.ViewModels;

namespace PlotTwistLibrary;

public partial class UserDashboardWindow : Window
{
    private readonly Member _member;
    private readonly LibrarySystem _librarySystem;

    public UserDashboardWindow(Member member)
    {
        InitializeComponent();

        _member = member;
        _librarySystem = new LibrarySystem();

        UserBadgeText.Text = $"User: {_member.FirstName}";
        WelcomeText.Text = $"Welcome back, {_member.FirstName} — what adventure are we on today?";

        LoadBorrowedBooks();
    }

    private void LoadBorrowedBooks()
    {
        // Active loans for this member
        var memberLoans = _librarySystem.Loans
            .Where(l => l.MemberId == _member.MemberId && !l.Returned)
            .ToList();

        var items = new List<BorrowedBookViewModel>();

        foreach (var loan in memberLoans)
        {
            var book = _librarySystem.Books.FirstOrDefault(b => b.BookId == loan.BookId);

            var vm = new BorrowedBookViewModel
            {
                BookId = loan.BookId,
                Title = book?.Title ?? "Unknown Book",
                DueDateString = loan.DueDate.ToString("dd/MM/yyyy"),
                Status = loan.IsOverdue() ? "Overdue" : "Active"
            };

            items.Add(vm);
        }

        BorrowedBooksGrid.ItemsSource = items;
    }
    // Helper method to open the SearchBooksWindow
    private void OpenSearchBooksWindow()
    {
        WindowHelpers.OpenSearchBooksWindow(this, _librarySystem, _member);
    }

    private void SearchBooksButton_Click(object sender, RoutedEventArgs e)
    {
        OpenSearchBooksWindow();
    }

    private void TopBarSearchBooksButton_Click(object sender, RoutedEventArgs e)
    {
        OpenSearchBooksWindow();
    }
    // Helper method to handle returning a book
    private void HandleReturnBook()
    {
        var selected = BorrowedBooksGrid.SelectedItem as BorrowedBookViewModel;

        if (selected != null)
        {
            // Confirmation for the selected book
            var message = $"You wish to return {selected.Title} due {selected.DueDateString}.";
            var result = MessageBox.Show(message, "Confirm return", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                // Find matching active loan for this member/book
                var loan = _librarySystem.Loans
    .FirstOrDefault(l => l.MemberId == _member.MemberId &&
                         l.BookId == selected.BookId &&
                         !l.Returned);

                if (loan != null)
                {
                    _librarySystem.ReturnLoan(loan);
                    LoadBorrowedBooks();     // refresh grid
                }
            }
            return;
        }

        // No selection: use popup
        var returnWindow = new ReturnBookWindow(_librarySystem, _member)
        {
            Owner = this
        };

        bool? dialogResult = returnWindow.ShowDialog();
        if (dialogResult == true)
        {
            var loan = returnWindow.SelectedLoan;
            if (loan != null)
            {
                _librarySystem.ReturnLoan(loan);
                LoadBorrowedBooks();         // refresh grid
            }
        }
    }
    // No selection: open the popup
     
    private void ReturnBookButton_Click(object sender, RoutedEventArgs e)
    {
        HandleReturnBook();
    }
    private void HeaderReturnBookButton_Click(object sender, RoutedEventArgs e)
    {
        HandleReturnBook();
    }
}
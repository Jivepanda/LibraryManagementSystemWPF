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
                Title = book?.Title ?? "Unknown Book",
                DueDateString = loan.DueDate.ToString("dd/MM/yyyy"),
                Status = loan.IsOverdue() ? "Overdue" : "Active"
            };

            items.Add(vm);
        }

        BorrowedBooksGrid.ItemsSource = items;
    }
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
}
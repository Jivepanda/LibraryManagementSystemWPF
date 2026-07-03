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

    public UserDashboardWindow(Member member, LibrarySystem librarySystem)
    {
        InitializeComponent();

        _member = member;
        _librarySystem = librarySystem;
        _librarySystem.UpdateReadyToCollectReservationsForMember(_member.MemberId);

        UserBadgeText.Text = $"User: {_member.FirstName}";
        WelcomeText.Text = $"Welcome back, {_member.FirstName} — what adventure are we on today?";

        LoadBorrowedBooks();
    }

    private void LoadBorrowedBooks()
    {
        var items = new List<BorrowedBookViewModel>();

        var memberLoans = _librarySystem.Loans
            .Where(l => l.MemberId == _member.MemberId && !l.Returned)
            .ToList();

        foreach (var loan in memberLoans)
        {
            var book = _librarySystem.Books.FirstOrDefault(b => b.BookId == loan.BookId);

            items.Add(new BorrowedBookViewModel
            {
                BookId = loan.BookId,
                LoanId = loan.LoanId,
                Title = book?.Title ?? "Unknown Book",
                DateString = loan.DueDate.ToString("dd/MM/yyyy"),
                Status = loan.IsOverdue() ? "Overdue" : "Active"
            });
        }

        var memberReservations = _librarySystem.Reservations
            .Where(r => r.MemberId == _member.MemberId && r.IsActive)
            .ToList();

        foreach (var reservation in memberReservations)
        {
            var book = _librarySystem.Books.FirstOrDefault(b => b.BookId == reservation.BookId);

            items.Add(new BorrowedBookViewModel
            {
                BookId = reservation.BookId,
                ReservationId = reservation.ReservationId,
                Title = book?.Title ?? "Unknown Book",
                DateString = reservation.ReserveExpiry.ToString("dd/MM/yyyy"),
                Status = reservation.Status == ReservationStatus.AvailableToCollect
                    ? "Available to Collect"
                    : "Reserved"
            });
        }

        BorrowedBooksGrid.ItemsSource = items;

        CollectReservedBookQuickMenuButton.Visibility =
            items.Any(i => i.IsReservationReadyToCollect)
                ? Visibility.Visible
                : Visibility.Collapsed;
    }
    private void OpenSearchBooksWindow()
    {
        var searchWindow = new SearchBooksWindow(_librarySystem, _member)
        {
            Owner = this
        };

        this.Hide();
        searchWindow.ShowDialog();
        this.Show();

        LoadBorrowedBooks();
    }
    private void BorrowBookButton_Click(object sender, RoutedEventArgs e)
    {
        OpenSearchBooksWindow();
    }

    private void SearchBooksButton_Click(object sender, RoutedEventArgs e) => OpenSearchBooksWindow();
    private void TopBarSearchBooksButton_Click(object sender, RoutedEventArgs e) => OpenSearchBooksWindow();
    private void ReserveBookQuickMenuButton_Click(object sender, RoutedEventArgs e) => OpenSearchBooksWindow();

    private void HandleCollectReservedBook()
    {
        var selected = BorrowedBooksGrid.SelectedItem as BorrowedBookViewModel;

        if (selected != null && selected.ReservationId.HasValue)
        {
            if (!selected.IsReservationReadyToCollect)
            {
                MessageBox.Show("The selected reservation is not ready to collect.", "Collect Reserved Book");
                return;
            }

            var message = $"You wish to collect \"{selected.Title}\".";
            if (MessageBox.Show(message, "Confirm Collection", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;

            var result = _librarySystem.CollectReservedBook(_member.MemberId, selected.ReservationId.Value);
            MessageBox.Show(result, "Collect Reserved Book");
            LoadBorrowedBooks();
            return;
        }

        var collectWindow = new CollectReservedBookWindow(_librarySystem, _member)
        {
            Owner = this
        };

        if (collectWindow.ShowDialog() == true && collectWindow.SelectedReservation != null)
        {
            var result = _librarySystem.CollectReservedBook(
                _member.MemberId,
                collectWindow.SelectedReservation.ReservationId);

            MessageBox.Show(result, "Collect Reserved Book");
            LoadBorrowedBooks();
        }
    }

    private void CollectReservedBookQuickMenuButton_Click(object sender, RoutedEventArgs e) => HandleCollectReservedBook();

    private void HandleReturnBook()
    {
        var selected = BorrowedBooksGrid.SelectedItem as BorrowedBookViewModel;

        if (selected != null)
        {
            if (!selected.IsActiveLoan || !selected.LoanId.HasValue)
            {
                MessageBox.Show("Please select an active loan to return.", "Return Book");
                return;
            }

            var message = $"You wish to return \"{selected.Title}\" due {selected.DateString}.";
            if (MessageBox.Show(message, "Confirm Return", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;

            var result = _librarySystem.ReturnLoan(selected.LoanId.Value);
            MessageBox.Show(result, "Return Book");
            LoadBorrowedBooks();
            return;
        }

        var returnWindow = new ReturnBookWindow(_librarySystem, _member)
        {
            Owner = this
        };

        if (returnWindow.ShowDialog() == true && returnWindow.SelectedLoan != null)
        {
            var result = _librarySystem.ReturnLoan(returnWindow.SelectedLoan.LoanId);
            MessageBox.Show(result, "Return Book");
            LoadBorrowedBooks();
        }
    }

    private void ReturnBookButton_Click(object sender, RoutedEventArgs e) => HandleReturnBook();
    private void HeaderReturnBookButton_Click(object sender, RoutedEventArgs e) => HandleReturnBook();
    private void LogoutButton_Click(object sender, RoutedEventArgs e)
    {
        var loginWindow = new LoginWindow();
        loginWindow.Show();
        this.Close();
    }
}
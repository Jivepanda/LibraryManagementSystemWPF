using System;
using System.Windows;
using System.Windows.Input;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services;

namespace PlotTwistLibrary
{
    public partial class SearchBooksWindow : Window
    {
        private readonly LibrarySystem _librarySystem;
        private readonly Member _member;

        public SearchBooksWindow(LibrarySystem librarySystem, Member member)
        {
            InitializeComponent();

            _librarySystem = librarySystem;
            _member = member;

            UserBadgeText.Text = $"User: {_member.FirstName}";
        }

        private void RunSearch()
        {
            var query = SearchTextBox.Text?.Trim() ?? string.Empty;
            var results = _librarySystem.SearchBooks(query);
            SearchResultsGrid.ItemsSource = results;
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            RunSearch();
        }

        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                RunSearch();
            }
        }
        private void BorrowBookButton_Click(object sender, RoutedEventArgs e)
        {
            if (SearchResultsGrid.SelectedItem is not Book selectedBook)
            {
                MessageBox.Show("Please select a book to borrow.", "Borrow Book",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int memberIdToBorrowFor = _member.MemberId;

            if (string.Equals(_member.Role, "Staff", StringComparison.OrdinalIgnoreCase))
            {
                var borrowWindow = new BorrowForMemberWindow()
                {
                    Owner = this
                };

                bool? dialogResult = borrowWindow.ShowDialog();
                if (dialogResult == true && borrowWindow.SelectedMemberId.HasValue)
                {
                    memberIdToBorrowFor = borrowWindow.SelectedMemberId.Value;
                }
                else
                {
                    return;
                }
            }

            string result = _librarySystem.BorrowBook(memberIdToBorrowFor, selectedBook.BookId);

            MessageBox.Show(result, "Borrow Book",
                MessageBoxButton.OK, MessageBoxImage.Information);

            var query = SearchTextBox.Text?.Trim() ?? string.Empty;
            SearchResultsGrid.ItemsSource = _librarySystem.SearchBooks(query);
        }

        private void ReserveSelectedButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedBook = SearchResultsGrid.SelectedItem as Book;

            if (selectedBook == null)
            {
                MessageBox.Show("Please select a book to reserve.", "Reserve Book",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int memberIdToReserveFor = _member.MemberId;

            if (string.Equals(_member.Role, "Staff", StringComparison.OrdinalIgnoreCase))
            {
                var reserveWindow = new BorrowForMemberWindow
                {
                    Owner = this
                };

                bool? dialogResult = reserveWindow.ShowDialog();

                if (dialogResult == true && reserveWindow.SelectedMemberId.HasValue)
                {
                    memberIdToReserveFor = reserveWindow.SelectedMemberId.Value;
                }
                else
                {
                    return;
                }
            }

            string result = _librarySystem.ReservationsManager
                .ReserveBook(memberIdToReserveFor, selectedBook.BookId);

            MessageBox.Show(result, "Reserve Book",
                MessageBoxButton.OK, MessageBoxImage.Information);

            var query = SearchTextBox.Text?.Trim() ?? string.Empty;
            SearchResultsGrid.ItemsSource = _librarySystem.SearchBooks(query);
        }
        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            if (Owner != null)
            {
                Owner.Show();
                Owner.Activate();
            }

            Close();
        }
        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();

            if (Owner != null)
            {
                Owner.Close();
            }

            Close();
        }

        private void SearchResultsGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }
    }
}
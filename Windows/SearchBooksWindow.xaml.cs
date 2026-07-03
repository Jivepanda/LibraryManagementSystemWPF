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

            string result = _librarySystem.BorrowBook(_member.MemberId, selectedBook.BookId);

            MessageBox.Show(result, "Borrow Book",
                MessageBoxButton.OK, MessageBoxImage.Information);

            var query = SearchTextBox.Text?.Trim() ?? string.Empty;
            SearchResultsGrid.ItemsSource = _librarySystem.SearchBooks(query);
        }

        private void ReserveBookButton_Click(object sender, RoutedEventArgs e)
        {
            if (SearchResultsGrid.SelectedItem is not Book selectedBook)
            {
                MessageBox.Show("Please select a book to reserve.", "Reserve Book",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = _librarySystem.ReservationsManager.ReserveBook(_member.MemberId, selectedBook.BookId);
            MessageBox.Show(result, "Reserve Book", MessageBoxButton.OK, MessageBoxImage.Information);

            RunSearch();
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
    }
}
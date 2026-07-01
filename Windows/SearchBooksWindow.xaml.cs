using System.Windows;
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

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            LoadSearchResults();
        }

        private void ReserveBookButton_Click(object sender, RoutedEventArgs e)
        {
            if (SearchResultsGrid.SelectedItem is not Book selectedBook)
            {
                MessageBox.Show("Please select a book to reserve.", "Reserve Book");
                return;
            }

            string result = _librarySystem.ReservationsManager
                .ReserveBook(_member.MemberId, selectedBook.BookId);

            MessageBox.Show(result, "Reserve Book");
            LoadSearchResults();
        }

        private void LoadSearchResults()
        {
            var query = SearchTextBox.Text?.Trim() ?? string.Empty;
            var results = _librarySystem.SearchBooks(query);
            SearchResultsGrid.ItemsSource = results;
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
    }
}
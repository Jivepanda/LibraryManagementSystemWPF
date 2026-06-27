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
            var query = SearchTextBox.Text?.Trim() ?? string.Empty;
            var results = _librarySystem.SearchBooks(query);
            SearchResultsGrid.ItemsSource = results;
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            // If this search window was opened by a parent window
            if (this.Owner != null)
            {
                // Bring the user/staff window back to the front
                this.Owner.Show();
                this.Owner.Activate();
            }

            // Close the search window
            this.Close();
        }
    }
}
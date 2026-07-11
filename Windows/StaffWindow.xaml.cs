using System.Windows;
using LibraryManagementSystem.Services;
using PlotTwistLibrary;

namespace LibraryManagementSystem.Windows
{
    public partial class StaffWindow : Window
    {
        private readonly LibrarySystem _librarySystem;
        private readonly Member _staffMember;

        public StaffWindow(LibrarySystem librarySystem, Member staffMember)
        {
            InitializeComponent();
            _librarySystem = librarySystem;
            _staffMember = staffMember;
        }
        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Home clicked.");
        }

        private void BooksButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Books screen coming next.");
        }

        private void LoansButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Loans screen coming next.");
        }

        private void UsersButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Users screen coming next.");
        }

        private void ReportsButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Reports screen coming next.");
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SearchBooksButton_Click(object sender, RoutedEventArgs e)
        {
            var searchWindow = new SearchBooksWindow(_librarySystem, _staffMember)
            {
                Owner = this
            };

            searchWindow.ShowDialog();
        }

        private void ViewAllBorrowedBooksButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("View All Borrowed Books clicked.");
        }

        private void ViewBorrowingUsersButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("View Borrowing Users clicked.");
        }

        private void AddEditBooksButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Add / Edit Books clicked.");
        }
    }
}
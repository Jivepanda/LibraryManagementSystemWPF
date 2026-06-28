using System.Linq;
using System.Windows;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services;

namespace PlotTwistLibrary
{
    public partial class ReturnBookWindow : Window
    {
        private readonly LibrarySystem _librarySystem;
        private readonly Member _member;

        public Loan SelectedLoan { get; private set; }

        private class LoanDisplay
        {
            public string Title { get; set; }
            public Loan Loan { get; set; }
        }

        public ReturnBookWindow(LibrarySystem librarySystem, Member member)
        {
            InitializeComponent();
            _librarySystem = librarySystem;
            _member = member;
            LoadActiveLoans();
        }

        private void LoadActiveLoans()
        {
            var items = _librarySystem.Loans
                .Where(l => l.MemberId == _member.MemberId && !l.Returned)
                .Select(l => new LoanDisplay
                {
                    Loan = l,
                    Title = _librarySystem.Books
                        .FirstOrDefault(b => b.BookId == l.BookId)?.Title ?? "Unknown"
                })
                .ToList();

            LoansListBox.ItemsSource = items;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = LoansListBox.SelectedItem as LoanDisplay;
            SelectedLoan = selected?.Loan;

            if (SelectedLoan != null)
            {
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Please select a loan to return.", "No Selection",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
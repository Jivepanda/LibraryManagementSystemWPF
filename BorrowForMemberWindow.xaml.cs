using System.Windows;

namespace PlotTwistLibrary
{
    public partial class BorrowForMemberWindow : Window
    {
        public int? SelectedMemberId { get; private set; }

        public BorrowForMemberWindow()
        {
            InitializeComponent();
        }

        private void BorrowButton_Click(object sender, RoutedEventArgs e)
        {
            var text = MemberIdTextBox.Text?.Trim();

            if (!int.TryParse(text, out int memberId))
            {
                MessageBox.Show("Please enter a valid numeric Member ID.",
                    "Borrow Book",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            SelectedMemberId = memberId;
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
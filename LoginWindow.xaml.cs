using System.Windows;
using System.Windows.Input;

namespace PlotTwistLibrary;

public partial class LoginWindow : Window
{
    public LoginWindow()
    {
        InitializeComponent();
    }

    private void SignInButton_Click(object sender, RoutedEventArgs e)
    {
        string memberId = MemberIdTextBox.Text.Trim();

        // TODO: validate memberId against your data source
        if (!string.IsNullOrEmpty(memberId))
        {
            // e.g. open main window
            // new MainWindow(memberId).Show();
            // this.Close();
        }
        else
        {
            MessageBox.Show("Please enter your Member ID.", "Login", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void RegisterText_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        // TODO: open registration window
        // new RegisterWindow().Show();
    }
}
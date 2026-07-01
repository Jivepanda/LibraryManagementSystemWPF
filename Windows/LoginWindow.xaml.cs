using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using LibraryManagementSystem.Services;
using LibraryManagementSystem.Models;

namespace PlotTwistLibrary;

public partial class LoginWindow : Window
{
    private readonly LibrarySystem _librarySystem;

    public LoginWindow()
    {
        InitializeComponent();
        _librarySystem = new LibrarySystem();
    }

    private void SignInButton_Click(object sender, RoutedEventArgs e)
    {
        var text = MemberIdTextBox.Text?.Trim();

        if (!int.TryParse(text, out int memberId))
        {
            MessageBox.Show("Please enter a valid numeric Member ID.", "Login",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var member = _librarySystem.Members.FirstOrDefault(m => m.MemberId == memberId);

        if (member == null)
        {
            MessageBox.Show("Member not found. Please check your ID or register first.", "Login",
                MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        if (!string.Equals(member.Role, "Staff", StringComparison.OrdinalIgnoreCase))
        {
            var dashboard = new UserDashboardWindow(member, _librarySystem);
            dashboard.Show();
            Close();
        }
        else
        {
            MessageBox.Show("This ID belongs to staff. Please use the staff login path.",
                "Login", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private void MemberIdTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            SignInButton_Click(SignInButton, new RoutedEventArgs());
        }
    }

    private void RegisterText_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        // TODO: open registration window
    }
}
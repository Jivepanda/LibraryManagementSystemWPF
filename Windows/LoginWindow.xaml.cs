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

        if (string.Equals(member.Role, "Staff", StringComparison.OrdinalIgnoreCase))
        {
            var staffWindow = new LibraryManagementSystem.Windows.StaffWindow(_librarySystem, member);
            staffWindow.Show();
            this.Close();
        }
        else
        {
            var dashboard = new UserDashboardWindow(member, _librarySystem);
            dashboard.Show();
            this.Close();
        }
    }

    private void MemberIdTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            SignInButton_Click(SignInButton, new RoutedEventArgs());
        }
    }

    private void RegisterText_MouseLeftButtonUp(object sender, RoutedEventArgs e)
    {
        var registerWindow = new RegisterMemberWindow(_librarySystem)
        {
            Owner = this
        };

        bool? result = registerWindow.ShowDialog();

        if (result == true && registerWindow.RegisteredMember != null)
        {
            MemberIdTextBox.Text = registerWindow.RegisteredMember.MemberId.ToString();
            MemberIdTextBox.Focus();
            MemberIdTextBox.SelectAll();
        }
    }
}
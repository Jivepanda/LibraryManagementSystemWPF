using System;
using System.Windows;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services;

namespace PlotTwistLibrary;

public partial class RegisterMemberWindow : Window
{
    private readonly LibrarySystem _librarySystem;

    public Member? RegisteredMember { get; private set; }

    public RegisterMemberWindow(LibrarySystem librarySystem)
    {
        InitializeComponent();
        _librarySystem = librarySystem;
    }

    private void RegisterButton_Click(object sender, RoutedEventArgs e)
    {
        string firstName = FirstNameTextBox.Text.Trim();
        string lastName = LastNameTextBox.Text.Trim();
        string email = EmailTextBox.Text.Trim();
        string phone = PhoneTextBox.Text.Trim();

        if (string.IsNullOrWhiteSpace(firstName) ||
            string.IsNullOrWhiteSpace(lastName) ||
            string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(phone))
        {
            MessageBox.Show("Please fill in all fields.", "Registration Error",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            RegisteredMember = _librarySystem.RegisterMember(firstName, lastName, email, phone);

            MessageBox.Show(
                $"Registration successful! Your Member ID is {RegisteredMember.MemberId}.",
                "Success",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            DialogResult = true;
            Close();
        }
        catch (InvalidOperationException ex)
        {
            MessageBox.Show(ex.Message, "Registration Error",
                MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        catch (Exception)
        {
            MessageBox.Show("Something went wrong while registering the member.",
                "Registration Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
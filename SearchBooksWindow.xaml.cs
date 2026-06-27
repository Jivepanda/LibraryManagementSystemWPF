using System;
using System.Linq;
using System.Windows;
using LibraryManagementSystem.Services;
using LibraryManagementSystem.Models;

namespace PlotTwistLibrary;

public partial class SearchBooksWindow : Window
{
    private readonly LibrarySystem _librarySystem;

    public SearchBooksWindow(LibrarySystem librarySystem)
    {
        InitializeComponent();
        _librarySystem = librarySystem;
    }

    private void SearchButton_Click(object sender, RoutedEventArgs e)
    {
        var query = SearchTextBox.Text?.Trim() ?? string.Empty;

        var results = _librarySystem.SearchBooks(query);

        SearchResultsGrid.ItemsSource = results;
    }
}
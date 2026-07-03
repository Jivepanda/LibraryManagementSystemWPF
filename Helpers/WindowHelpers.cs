using System.Windows;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services;
using PlotTwistLibrary;

namespace LibraryManagementSystem.Helpers
{
    public static class WindowHelpers
    {
        public static void OpenSearchBooksWindow(Window owner, LibrarySystem librarySystem, Member member)
        {
            var searchWindow = new SearchBooksWindow(librarySystem, member)
            {
                Owner = owner
            };

            owner.Hide();
            searchWindow.ShowDialog();
            owner.Show();
        }
    }
}
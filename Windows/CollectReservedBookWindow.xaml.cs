using System.Linq;
using System.Windows;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services;

namespace PlotTwistLibrary
{
    public partial class CollectReservedBookWindow : Window
    {
        private readonly LibrarySystem _librarySystem;
        private readonly Member _member;

        public Reservation? SelectedReservation { get; private set; }

        private class ReservationDisplay
        {
            public string Title { get; set; } = string.Empty;
            public Reservation Reservation { get; set; } = null!;
        }

        public CollectReservedBookWindow(LibrarySystem librarySystem, Member member)
        {
            InitializeComponent();
            _librarySystem = librarySystem;
            _member = member;
            LoadCollectibleReservations();
        }

        private void LoadCollectibleReservations()
        {
            var items = _librarySystem.Reservations
                .Where(r => r.MemberId == _member.MemberId &&
                            r.Status == ReservationStatus.AvailableToCollect)
                .Select(r => new ReservationDisplay
                {
                    Reservation = r,
                    Title = _librarySystem.Books
                        .FirstOrDefault(b => b.BookId == r.BookId)?.Title ?? "Unknown"
                })
                .ToList();

            ReservationsListBox.ItemsSource = items;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = ReservationsListBox.SelectedItem as ReservationDisplay;
            SelectedReservation = selected?.Reservation;

            if (SelectedReservation != null)
            {
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Please select a reservation to collect.", "No Selection",
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
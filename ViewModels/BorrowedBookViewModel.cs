namespace LibraryManagementSystem.ViewModels
{
    public class BorrowedBookViewModel
    {
        public string Title { get; set; } = string.Empty;
        public string DueDateString { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
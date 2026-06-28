namespace LibraryManagementSystem.ViewModels
{
    public class BorrowedBookViewModel
    {
        public int BookId { get; set; }          // internal use only

        public string Title { get; set; }
        public string DueDateString { get; set; }
        public string Status { get; set; }
    }
}
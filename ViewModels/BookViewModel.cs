namespace WPF_Library.ViewModels
{
    public class BookViewModel : BaseViewModel
    {
        private string _title { get; set; }
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }
        private string _authorName { get; set; }
        public string AuthorName
        {
            get => _authorName;
            set
            {
                _authorName = value;
                OnPropertyChanged(nameof(AuthorName));
            }
        }
        private string _authorSurname { get; set; }
        public string AuthorSurname
        {
            get => _authorSurname;
            set
            {
                _authorSurname = value;
                OnPropertyChanged(nameof(AuthorSurname));
            }
        }
        private string _category { get; set; }
        public string Category
        {
            get => _category;
            set
            {
                _category = value;
                OnPropertyChanged(nameof(Category));
            }
        }
        private int _stockSize { get; set; }
        public int StockSize
        {
            get => _stockSize;
            set
            {
                _stockSize = value;
                OnPropertyChanged(nameof(StockSize));
            }
        }
    }
}

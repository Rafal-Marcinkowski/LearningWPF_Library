using System.Windows;
using WPF_Library.ViewModels;

namespace WPF_Library
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainViewModel _viewModel = new MainViewModel();
        public MainWindow()
        {
            InitializeComponent();
            DataContext = _viewModel;
            _viewModel.BookCollection = DataBaseInteraction.GetAvailableBookCollectionFromDB();
            availaibleBooksListView.ItemsSource = _viewModel.BookCollection;
            borrowedBooksListView.ItemsSource = _viewModel.BorrowedBooksCollection;
            _viewModel.NumberOfRegisteredAcc = DataBaseInteraction.GetNumberOfRegisteredAccInDB();
        }
    }
}

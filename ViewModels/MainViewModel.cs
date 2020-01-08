using System.Collections.ObjectModel;
using System.Windows.Input;
using WPF_ThirdParty.VM_Implementation;
using System.Linq;
using System.Windows;
using System;

namespace WPF_Library.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public ObservableCollection<BookViewModel> BookCollection = new ObservableCollection<BookViewModel>();

        public ObservableCollection<BookViewModel> BorrowedBooksCollection = new ObservableCollection<BookViewModel>();

        public ObservableCollection<PersonViewModel> PeopleCollection = new ObservableCollection<PersonViewModel>();
        public PersonViewModel Person { get; set; } = new PersonViewModel();
        public BookViewModel Book { get; set; } = new BookViewModel();
        public PersonViewModel PersonThatLogsIn { get; set; } = new PersonViewModel();
        private bool _isSomeoneLogged { get; set; } = false;
        public string LoggedPersonSurname { get; set; }
        public string LoggedPersonName { get; set; }
        public int SelectedBookID { get; set; }
        public int LoggedPersonID { get; set; }
        public bool IsSomeoneLogged
        {
            get => _isSomeoneLogged;
            set
            {
                _isSomeoneLogged = value;
                OnPropertyChanged(nameof(IsSomeoneLogged));
            }
        }
        private static int _numberOfRegisteredAcc { get; set; }
        public int NumberOfRegisteredAcc
        {
            get => _numberOfRegisteredAcc;
            set
            {
                _numberOfRegisteredAcc = value;
                OnPropertyChanged(nameof(NumberOfRegisteredAcc));
            }
        }
        string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }
        string _errorMessage2;
        public string ErrorMessage2
        {
            get => _errorMessage2;
            set
            {
                _errorMessage2 = value;
                OnPropertyChanged(nameof(ErrorMessage2));
            }
        }
        public ICommand RegisterUser =>
            new RelayCommand(() =>
            {
                if (!DataBaseInteraction.IsLoginCorrect(Person.Login) && !string.IsNullOrEmpty(Person.Login))
                {
                    ErrorMessage = "That Login is already taken.";
                    OnPropertyChanged(nameof(ErrorMessage));
                    return;
                }
                if (!(ValidateData(Person.Name, Person.Surname) && (!string.IsNullOrEmpty(Person.Login) && !string.IsNullOrEmpty(Person.Password))))
                {
                    ErrorMessage = "Invalid data.";
                    OnPropertyChanged(nameof(ErrorMessage));
                    return;
                }
                else
                {
                    PeopleCollection.Add(Person);
                    DataBaseInteraction.AddPersonToDB(Person);
                    Person = new PersonViewModel();
                    OnPropertyChanged(nameof(Person));
                    NumberOfRegisteredAcc++;
                    OnPropertyChanged(nameof(NumberOfRegisteredAcc));
                    ErrorMessage = "Account succesfully registered.";
                    OnPropertyChanged(nameof(ErrorMessage));
                }
            }, () => IsSomeoneLogged == false);
        public ICommand LogInCommand =>
            new RelayCommand(() =>
            {
                if ((!DataBaseInteraction.IsPersonInDB(PersonThatLogsIn.Login, PersonThatLogsIn.Password)))
                {
                    ErrorMessage2 = "Invalid data.";
                    OnPropertyChanged(nameof(ErrorMessage2));
                    return;
                }
                else
                {
                    ErrorMessage2 = "Logged in successfully.";
                    OnPropertyChanged(nameof(ErrorMessage2));
                    ErrorMessage = null;
                    OnPropertyChanged(nameof(ErrorMessage));
                    Person = new PersonViewModel();
                    OnPropertyChanged(nameof(Person));
                    IsSomeoneLogged = true;
                    OnPropertyChanged(nameof(IsSomeoneLogged));
                    ObservableCollection<BookViewModel> listOfBorrowedBooksFromDB = new ObservableCollection<BookViewModel>();
                    LoggedPersonName = DataBaseInteraction.LoggedPersonName(PersonThatLogsIn.Login, PersonThatLogsIn.Password);
                    LoggedPersonSurname = DataBaseInteraction.LoggedPersonSurname(PersonThatLogsIn.Login, PersonThatLogsIn.Password);
                    OnPropertyChanged(nameof(LoggedPersonName));
                    OnPropertyChanged(nameof(LoggedPersonSurname));
                    LoggedPersonID = DataBaseInteraction.LoggedPersonID(PersonThatLogsIn.Login, PersonThatLogsIn.Password);
                    listOfBorrowedBooksFromDB = DataBaseInteraction.GetListOfBorrowedBooks(LoggedPersonID);
                    foreach (BookViewModel book in listOfBorrowedBooksFromDB)
                    {
                        BorrowedBooksCollection.Add(book);
                        OnPropertyChanged(nameof(BorrowedBooksCollection));
                    }
                    PersonThatLogsIn = new PersonViewModel();
                    OnPropertyChanged(nameof(PersonThatLogsIn));
                }
            }, () => IsSomeoneLogged == false);
        public ICommand LogOutCommand =>
            new RelayCommand(() =>
            {
                IsSomeoneLogged = false;
                BorrowedBooksCollection.Clear();
                OnPropertyChanged(nameof(BorrowedBooksCollection));
                LoggedPersonSurname = null;
                LoggedPersonName = null;
                LoggedPersonID = 0;
                SelectedBookID = 0;
                OnPropertyChanged(nameof(LoggedPersonName));
                OnPropertyChanged(nameof(LoggedPersonSurname));
                ErrorMessage2 = null;
                OnPropertyChanged(nameof(ErrorMessage2));
            }, () => IsSomeoneLogged);
        public ICommand AddBookToDBCommand =>
            new RelayCommand(() =>
            {
                SelectedBookID = DataBaseInteraction.SelectedBookID(Book);
                if (DataBaseInteraction.IsThisBookAlreadyInDB(SelectedBookID))
                {
                    DataBaseInteraction.AddBookToDB(SelectedBookID, Book);
                    var b = BookCollection.FirstOrDefault(item => (item.Title == Book.Title && item.AuthorName == Book.AuthorName && item.AuthorSurname == Book.AuthorSurname && item.Category == Book.Category));
                    b.StockSize += Book.StockSize;
                    OnPropertyChanged(nameof(BookCollection));
                }
                else
                {
                    BookCollection.Add(Book);
                    OnPropertyChanged(nameof(BookCollection));
                    DataBaseInteraction.AddBookToDB(SelectedBookID, Book);
                }
                Book = new BookViewModel();
                OnPropertyChanged(nameof(Book));
            }, () => Book.StockSize > 0 && !string.IsNullOrEmpty(Book.AuthorName) && !string.IsNullOrEmpty(Book.AuthorSurname) && !string.IsNullOrEmpty(Book.Category) && !string.IsNullOrEmpty(Book.Title));
        public ICommand BorrowSelectedBookCommand =>
            new RelayCommand<BookViewModel>((bookToBorrow) =>
            {
                if (bookToBorrow == null)
                    return;
                SelectedBookID = DataBaseInteraction.SelectedBookID(bookToBorrow);
                if (bookToBorrow.StockSize == 0)
                {
                    MessageBox.Show("Book is out of stock, sorry!");
                }
                else if (DataBaseInteraction.IsThisKindOfBookAlreadyBorrowedByUser(SelectedBookID, LoggedPersonID))
                {
                    MessageBox.Show("You already have this kind of book, turn back previous one first!");
                }
                else
                {
                    DataBaseInteraction.AddNewBorrowRelation(SelectedBookID, LoggedPersonID);
                    DataBaseInteraction.SubstractBorrowedBookFromDB(SelectedBookID);
                    bookToBorrow.StockSize -= 1;
                    OnPropertyChanged(nameof(BookCollection));
                    BorrowedBooksCollection.Add(bookToBorrow);
                    OnPropertyChanged(nameof(BorrowedBooksCollection));
                }
            }, (bookToBorrow) => bookToBorrow != null && LoggedPersonID != 0);
        public ICommand TurnBackSelectedBook =>
            new RelayCommand<BookViewModel>((bookToTurnBack) =>
            {
                SelectedBookID = DataBaseInteraction.SelectedBookID(bookToTurnBack);
                int stockSizeNumberBufor = bookToTurnBack.StockSize;
                bookToTurnBack.StockSize = 1;
                DataBaseInteraction.AddBookToDB(SelectedBookID, bookToTurnBack);
                bookToTurnBack.StockSize = stockSizeNumberBufor;
                var b = BookCollection.FirstOrDefault(item => (item.Title == bookToTurnBack.Title && item.AuthorName == bookToTurnBack.AuthorName && item.AuthorSurname == bookToTurnBack.AuthorSurname && item.Category == bookToTurnBack.Category));
                b.StockSize += 1;
                OnPropertyChanged(nameof(BookCollection));
                BorrowedBooksCollection.Remove(bookToTurnBack);
                OnPropertyChanged(nameof(BorrowedBooksCollection));
                DataBaseInteraction.DeleteBorrowRelationship(LoggedPersonID, SelectedBookID);
            }, (bookToTurnBack) => bookToTurnBack != null && LoggedPersonID != 0);
        public bool ValidateData(string firstValue, string secondValue)
        {
            if (string.IsNullOrEmpty(firstValue) || string.IsNullOrEmpty(secondValue))
                return false;
            return (firstValue.All(q => Char.IsLetter(q)) &&
                      secondValue.All(q => Char.IsLetter(q)));
        }
    }
}

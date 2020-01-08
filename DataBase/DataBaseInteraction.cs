using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Windows;

namespace WPF_Library.ViewModels
{
    public static class DataBaseInteraction
    {
        public static SqlCommand command = new SqlCommand();
        private static string connectionString = "Data Source = MIETOSZEW\\SQLEXPRESS; Initial Catalog = Library; Integrated Security = True";
        private static SqlConnection connection = new SqlConnection(connectionString);
        public static bool IsLoginCorrect(string login)
        {
            string queryString = $@"SELECT ISNULL((SELECT personid FROM people WHERE personlogin=@login),0)";
            command = new SqlCommand(queryString, connection);
            command.Parameters.AddWithValue("@login", login);
            connection.Open();
            try
            {
                return Convert.ToInt32(command.ExecuteScalar()) == 0;
            }
            catch
            {
                return false;
            }
            finally
            {
                connection.Close();
            }
        }
        public static void AddPersonToDB(PersonViewModel person)
        {
            string queryString = $"INSERT INTO people VALUES('{person.Name}', '{person.Surname}', '{person.Login}', '{person.Password}')";
            connection.Open();
            SqlTransaction transaction = connection.BeginTransaction();
            command = new SqlCommand(queryString, connection);
            command.Transaction = transaction;
            command.Connection = connection;
            try
            {
                command.ExecuteNonQuery();
                transaction.Commit();
            }
            catch
            {
                try
                {
                    transaction.Rollback();
                }
                catch
                {
                    MessageBox.Show("Something went wrong.");
                    return;
                }
            }
            finally
            {
                connection.Close();
            }
        }
        public static void AddBookToDB(int bookid, BookViewModel book)
        {
            if (IsThisBookAlreadyInDB(bookid))
            {
                string queryString = $"UPDATE books SET stock+={book.StockSize} WHERE bookid={bookid}";
                command = new SqlCommand(queryString, connection);
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                command.Connection = connection;
                command.Transaction = transaction;
                try
                {
                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch
                {
                    try
                    {
                        transaction.Rollback();
                    }
                    catch
                    {
                        MessageBox.Show("Someting went wrong.");
                    }
                }
                finally
                {
                    connection.Close();
                }
            }
            else
            {
                string queryString = $"INSERT INTO books VALUES('{book.AuthorName}','{book.AuthorSurname}','{book.Title}','{book.Category}',{book.StockSize})";
                command = new SqlCommand(queryString, connection);
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                command.Connection = connection;
                command.Transaction = transaction;
                try
                {
                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch
                {
                    try
                    {
                        transaction.Rollback();
                    }
                    catch
                    {
                        MessageBox.Show("Someting went wrong.");
                    }
                }
                finally
                {
                    connection.Close();
                }
            }
        }
        public static void SubstractBorrowedBookFromDB(int bookid)
        {
            string queryString = $"UPDATE books SET stock=stock-1 WHERE bookid=@bookid";
            command = new SqlCommand(queryString, connection);
            command.Parameters.AddWithValue("@bookid", bookid);
            connection.Open();
            SqlTransaction transaction = connection.BeginTransaction();
            command.Connection = connection;
            command.Transaction = transaction;
            try
            {
                command.ExecuteNonQuery();
                transaction.Commit();
            }
            catch
            {
                try
                {
                    transaction.Rollback();
                }
                catch
                {
                    MessageBox.Show("Someting went wrong.");
                }
            }
            finally
            {
                connection.Close();
            }
        }
        public static bool IsThisBookAlreadyInDB(int bookid)
        {
            string queryString = $@"SELECT ISNULL((SELECT bookid FROM books WHERE bookid=@bookid),0)";
            command = new SqlCommand(queryString, connection);
            command.Parameters.AddWithValue("@bookid", bookid);
            connection.Open();
            try
            {
                return Convert.ToInt32(command.ExecuteScalar()) > 0;
            }
            catch
            {
                return false;
            }
            finally
            {
                connection.Close();
            }
        }
        public static bool IsPersonInDB(string login, string password)
        {
            string queryString = $@"SELECT ISNULL((SELECT personid from PEOPLE WHERE personlogin='{login}' AND personpassword='{password}'),0)";
            connection.Open();
            command = new SqlCommand(queryString, connection);
            try
            {
                return Convert.ToInt32(command.ExecuteScalar()) > 0;
            }
            catch
            {
                return false;
            }
            finally
            {
                connection.Close();
            }
        }
        public static string LoggedPersonName(string login, string password)
        {
            string queryString = $"SELECT personname FROM people WHERE personlogin='{login}' AND personpassword='{password}'";
            connection.Open();
            command = new SqlCommand(queryString, connection);
            try
            {
                return Convert.ToString(command.ExecuteScalar());
            }
            catch
            {
                return null;
            }
            finally
            {
                connection.Close();
            }
        }
        public static string LoggedPersonSurname(string login, string password)
        {
            string queryString = $"SELECT personsurname FROM people WHERE personlogin='{login}' AND personpassword='{password}'";
            connection.Open();
            command = new SqlCommand(queryString, connection);
            try
            {
                return Convert.ToString(command.ExecuteScalar());
            }
            catch
            {
                return null;
            }
            finally
            {
                connection.Close();
            }
        }
        public static int LoggedPersonID(string login, string password)
        {
            string queryString = $@"SELECT ISNULL((SELECT personid FROM people WHERE personlogin='{login}' AND personpassword='{password}'),0)";
            command = new SqlCommand(queryString, connection);
            connection.Open();
            try
            {
                return (int)command.ExecuteScalar();
            }
            catch
            {
                return 0;
            }
            finally
            {
                connection.Close();
            }
        }
        public static int SelectedBookID(BookViewModel book)
        {
            connection.Open();
            string queryString = $@"SELECT ISNULL((SELECT bookid FROM books WHERE authorname='{book.AuthorName}' AND authorsurname='{book.AuthorSurname}' AND title='{book.Title}' AND category='{book.Category}'),0)";
            command = new SqlCommand(queryString, connection);
            try
            {
                return (int)command.ExecuteScalar();
            }
            catch
            {
                return 0;
            }
            finally
            {
                connection.Close();
            }
        }
        public static void AddNewBorrowRelation(int bookid, int personid)
        {
            string queryString = $"INSERT INTO borrows VALUES({personid},{bookid})";
            command = new SqlCommand(queryString, connection);
            connection.Open();
            SqlTransaction transaction = connection.BeginTransaction();
            command.Connection = connection;
            command.Transaction = transaction;
            try
            {
                command.ExecuteNonQuery();
                transaction.Commit();
            }
            catch
            {
                try
                {
                    transaction.Rollback();
                }
                catch
                {
                    MessageBox.Show("Something went wrong.");
                }
            }
            finally
            {
                connection.Close();
            }
        }
        public static void DeleteBorrowRelationship(int personid, int bookid)
        {
            string queryString = $"DELETE FROM borrows WHERE personid={personid} AND bookid={bookid}";
            command = new SqlCommand(queryString, connection);
            connection.Open();
            SqlTransaction transaction = connection.BeginTransaction();
            command.Connection = connection;
            command.Transaction = transaction;
            try
            {
                command.ExecuteNonQuery();
                transaction.Commit();
            }
            catch
            {
                try
                {
                    transaction.Rollback();
                }
                catch
                {
                    MessageBox.Show("Something went wrong.");
                }
            }
            finally
            {
                connection.Close();
            }
        }
        public static int GetNumberOfRegisteredAccInDB()
        {
            string queryString = $"SELECT personid FROM people";
            command = new SqlCommand(queryString, connection);
            connection.Open();
            int numberOfAcc = 0;
            SqlDataReader reader = command.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    numberOfAcc++;
                }
                return numberOfAcc;
            }
            catch
            {
                return 0;
            }
            finally
            {
                connection.Close();
            }
        }
        public static ObservableCollection<BookViewModel> GetAvailableBookCollectionFromDB()
        {
            string queryString = $"SELECT * FROM books";
            command = new SqlCommand(queryString, connection);
            ObservableCollection<BookViewModel> books = new ObservableCollection<BookViewModel>();
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    BookViewModel book = new BookViewModel();
                    book.AuthorName = reader.GetString(1);
                    book.AuthorSurname = reader.GetString(2);
                    book.Title = reader.GetString(3);
                    book.Category = reader.GetString(4);
                    book.StockSize = reader.GetInt32(5);
                    books.Add(book);
                }
                return books;
            }
            catch
            {
                MessageBox.Show("Something went wrong, while loading list of all available books");
                return books;
            }
            finally
            {
                connection.Close();
            }
        }
        public static ObservableCollection<BookViewModel> GetListOfBorrowedBooks(int loggedPersonID)
        {
            ObservableCollection<BookViewModel> borrowedBooks = new ObservableCollection<BookViewModel>();
            string queryString = $"SELECT bookid FROM borrows WHERE personid=@loggedPersonID";
            command = new SqlCommand(queryString, connection);
            command.Parameters.AddWithValue("@loggedPersonID", loggedPersonID);
            connection.Open();
            int number = 0;
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                number++;
            }
            if (number == 0)
            {
                connection.Close();
                return borrowedBooks;
            }
            int[] booksID = new int[number];
            connection.Close();
            connection.Open();
            reader = command.ExecuteReader();
            if (reader != null)
            {
                int tabCounter = 0;
                while (reader.Read())
                {
                    booksID[tabCounter] = reader.GetInt32(0);
                    tabCounter += 1;
                }
            }
            connection.Close();
            queryString = $@"SELECT * FROM books where bookid in ({String.Join(',', booksID)})";
            connection.Open();
            command = new SqlCommand(queryString, connection);
            reader = command.ExecuteReader();
            if (reader != null)
            {
                while (reader.Read())
                {
                    BookViewModel book = new BookViewModel
                    {
                        AuthorName = reader.GetString(1),
                        AuthorSurname = reader.GetString(2),
                        Title = reader.GetString(3),
                        Category = reader.GetString(4),
                        StockSize = reader.GetInt32(5)
                    };
                    borrowedBooks.Add(book);
                }
            }
            connection.Close();
            return borrowedBooks;
        }
        public static bool IsThisKindOfBookAlreadyBorrowedByUser(int bookid, int personid)
        {
            string queryString = $@"SELECT ISNULL((SELECT personid FROM borrows WHERE bookid={bookid} AND personid={personid}),0)";
            command = new SqlCommand(queryString, connection);
            connection.Open();
            try
            {
                return Convert.ToInt32(command.ExecuteScalar()) == personid;
            }
            catch
            {
                return false;
            }
            finally
            {
                connection.Close();
            }
        }
    }
}


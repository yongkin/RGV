using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DatabaseHelperApp;
using System.Data;
using System;

namespace LoginApp.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly DatabaseHelper _dbHelper;
        private const string DbProvider = "Microsoft.Data.Sqlite";
        private const string DbConnectionString = "Data Source=login.db";

        public MainWindow()
        {
            InitializeComponent();
            _dbHelper = new DatabaseHelper(DbProvider, DbConnectionString);
            SetupDatabase();
        }

        private void SetupDatabase()
        {
            // Create the table if it doesn't exist
            var createTableSql = @"
                CREATE TABLE IF NOT EXISTS UsersInfo (
                    UserName TEXT PRIMARY KEY,
                    Pwd TEXT NOT NULL,
                    LastLoginTime DATETIME
                );";
            _dbHelper.ExecuteNonQuery(createTableSql, CommandType.Text);

            // Check if there are any users. If not, add a default admin user.
            var userCount = _dbHelper.ExecuteScalar("SELECT COUNT(*) FROM UsersInfo", CommandType.Text);
            if (Convert.ToInt64(userCount) == 0)
            {
                // In a real application, passwords should be securely hashed and salted.
                var insertSql = "INSERT INTO UsersInfo (UserName, Pwd) VALUES (@UserName, @Pwd)";
                _dbHelper.ExecuteNonQuery(insertSql, CommandType.Text,
                    _dbHelper.CreateParameter("@UserName", "admin"),
                    _dbHelper.CreateParameter("@Pwd", "password"));
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var username = UsernameTextBox.Text;
            var password = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                StatusTextBlock.Text = "Username and password cannot be empty.";
                StatusTextBlock.Foreground = Brushes.Red;
                return;
            }
            //check password
            var selectSql = "SELECT Pwd FROM UsersInfo WHERE UserName = @UserName";
            var storedPassword = _dbHelper.ExecuteScalar(selectSql, CommandType.Text,
                                    _dbHelper.CreateParameter("@UserName", username));

            if (storedPassword != null && storedPassword.ToString() == password)
            {
                // Password matches. Update LastLoginTime.
                var updateSql = "UPDATE UsersInfo SET LastLoginTime = @LastLoginTime WHERE UserName = @UserName";
                _dbHelper.ExecuteNonQuery(updateSql, CommandType.Text,
                    _dbHelper.CreateParameter("@LastLoginTime", DateTime.Now),
                    _dbHelper.CreateParameter("@UserName", username));

                StatusTextBlock.Text = "Login successful!";
                StatusTextBlock.Foreground = Brushes.Green;
            }
            else
            {
                // Invalid credentials
                StatusTextBlock.Text = "Invalid username or password.";
                StatusTextBlock.Foreground = Brushes.Red;
            }
        }
    }
}

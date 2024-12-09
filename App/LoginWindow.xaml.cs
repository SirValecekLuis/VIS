using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;
using VIS.App;
using VIS;

namespace Vis.App
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }
        
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (PasswordBox != null && WatermarkText != null)
            {
                WatermarkText.Visibility = string.IsNullOrEmpty(PasswordBox.Password) ? Visibility.Visible : Visibility.Hidden;
            }
        }
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;
            
            var user = ValidateCredentials(username, password);
            
            if (user != null)
            {
                var mainDashboard = new MainDashboard(user);
                mainDashboard.Show();
                
                Close();
            }
            else
            {
                MessageBox.Show("Neplatné přihlašovací údaje!", "Chyba přihlášení", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private User? ValidateCredentials(string username, string password)
        {
            if (username == "" || password == "")
            {
                return null;
            }
            
            var db = DatabaseManager.Instance;
            var res = db.ExecuteQuery<User>("SELECT * FROM User WHERE Name = @username AND Password = @password",
                new SQLiteParameter("@username", username),
                new SQLiteParameter("@password", password));

            return res;
        }
    }
}
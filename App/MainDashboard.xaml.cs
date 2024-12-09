using System.Windows;

namespace VIS.App
{
    public partial class MainDashboard : Window
    {
        private User _loggedUser;

        public MainDashboard(User user)
        {
            InitializeComponent();

            _loggedUser = user;

            this.DataContext = _loggedUser;
        }

        private void AddUserButton_Click(object sender, RoutedEventArgs e)
        {
            var win = new AddUserToDb();

            win.ShowDialog();
        }

        private void EditUserButton_Click(object sender, RoutedEventArgs e)
        {
            var win = new EditUserFromDB();

            win.ShowDialog();
        }

        private void GenerateReportButton_Click(object sender, RoutedEventArgs e)
        {
            var win = new GenerateReport();

            win.ShowDialog();
        }

        private void ExecuteRemoteOperationButton_Click(object sender, RoutedEventArgs e)
        {
            var win = new MakeUserOperation(_loggedUser);

            win.ShowDialog();
        }
    }
}
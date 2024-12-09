using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace VIS.App
{
    public partial class EditUserFromDB : Window
    {
        public List<User> Users { get; set; }

        public EditUserFromDB()
        {
            InitializeComponent();
            LoadUsersToDataGrid();
        }

        private void LoadUsersToDataGrid()
        {
            try
            {
                Users = DatabaseManager.Instance.GetAllRecords<User>("User");

                UsersDataGrid.ItemsSource = Users;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nepodařilo se načíst uživatele: {ex.Message}", "Chyba", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void SaveUserButton_Click(object sender, RoutedEventArgs e)
        {
            if (UsersDataGrid.SelectedItem is not User selectedUser) return;

            if (string.IsNullOrWhiteSpace(selectedUser.Name) || string.IsNullOrWhiteSpace(selectedUser.Password))
            {
                MessageBox.Show("Jméno a heslo nesmí být prázdné!", "Chyba", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            try
            {
                DatabaseManager.Instance.UpdateRecord("User", selectedUser);

                MessageBox.Show("Změny byly úspěšně uloženy.", "Informace", MessageBoxButton.OK,
                    MessageBoxImage.Information);

                LoadUsersToDataGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nepodařilo se uložit změny: {ex.Message}", "Chyba", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}
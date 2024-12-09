using System;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;

namespace VIS.App;

public partial class AddUserToDb : Window
{
    public AddUserToDb()
    {
        InitializeComponent();
    }

    private void AddUserButton_Click(object sender, RoutedEventArgs e)
    {
        string name = NameTextBox.Text;
        string password = PasswordBox.Password;
        string role = RoleComboBox.SelectedItem is ComboBoxItem comboBoxItem ? comboBoxItem.Content.ToString() : null;

        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(role))
        {
            MessageBox.Show("Vyplňte prosím všechna pole.", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        try
        {
            var dbManager = DatabaseManager.Instance;
            
            var user = new User(dbManager.GetNextId("User"), name, password, role);
            dbManager.InsertRecord("User", user);
            dbManager.AddCSVRecord(user);
            
            MessageBox.Show($"Uživatel '{name}' byl úspěšně přidán.", "Úspěch", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Při přidávání uživatele došlo k chybě: {ex.Message} s errorem {ex.GetType()}", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        this.Close();
    }
}
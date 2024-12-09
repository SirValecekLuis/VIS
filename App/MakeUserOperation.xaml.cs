using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VIS;

namespace VIS.App
{
    public partial class MakeUserOperation : Window
    {
        public List<Device> Devices { get; set; }
        public List<Device> FilteredDevices { get; set; }

        private User _loggedUser;

        public MakeUserOperation(User user)
        {
            _loggedUser = user;
            InitializeComponent();
            LoadDevices();

            DevicesDataGrid.SelectedItem = DevicesDataGrid.SelectedItem;
        }

        private void LoadDevices()
        {
            var db = DatabaseManager.Instance;

            var devices = db.GetAllRecords<Device>("Device");

            Devices = devices ?? new List<Device>();

            FilteredDevices = new List<Device>(Devices);

            DevicesDataGrid.ItemsSource = FilteredDevices;
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var searchText = SearchTextBox.Text.ToLower();

            FilteredDevices = Devices
                .Where(d => d.Name.ToLower().Contains(searchText) || d.Location.ToLower().Contains(searchText))
                .ToList();

            DevicesDataGrid.ItemsSource = FilteredDevices;
        }

        private void ExecuteCommandButton_Click(object sender, RoutedEventArgs e)
        {
            if (DevicesDataGrid.SelectedItem is Device selectedDevice)
            {
                string command = CommandTextBox.Text;

                if (string.IsNullOrEmpty(command))
                {
                    MessageBox.Show("Musíte zadat příkaz.", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                try
                {
                    var userOperation = new UserOperation
                    {
                        Id = DatabaseManager.Instance.GetNextId("UserOperation"),
                        UserId = _loggedUser.Id,
                        DeviceId = selectedDevice.Id,
                        Command = command,
                        ExecTime = DateTime.Now
                    };

                    var db = DatabaseManager.Instance;

                    db.InsertRecord("UserOperation", userOperation);

                    MessageBox.Show("Příkaz byl úspěšně proveden.", "Úspěch", MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Došlo k chybě při provádění příkazu: {ex.Message}", "Chyba", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Musíte vybrat zařízení.", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
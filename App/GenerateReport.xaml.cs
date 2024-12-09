using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using VIS;

namespace VIS.App
{
    public partial class GenerateReport : Window
    {
        private List<Device> Devices { get; set; }
        private List<Device> FilteredDevices { get; set; }

        public GenerateReport()
        {
            InitializeComponent();
            LoadDevices();
        }

        private void LoadDevices()
        {
            var db = DatabaseManager.Instance;
            Devices = db.GetAllRecords<Device>("Device") ?? new List<Device>();
            FilteredDevices = new List<Device>(Devices);
            DevicesDataGrid.ItemsSource = FilteredDevices;
        }

        private void SearchTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            var searchText = SearchTextBox.Text.ToLower();
            FilteredDevices = Devices.Where(d =>
                d.Name.ToLower().Contains(searchText) || d.Location.ToLower().Contains(searchText)).ToList();
            DevicesDataGrid.ItemsSource = FilteredDevices;
        }

        private void ShowAverageButton_Click(object sender, RoutedEventArgs e)
        {
            if (DevicesDataGrid.SelectedItem is Device selectedDevice)
            {
                var db = DatabaseManager.Instance;
                var records = db.GetAllRecords<Record>("Record").Where(r => r.DeviceId == selectedDevice.Id).ToList();

                if (records.Any())
                {
                    var averageTemperature = records.Average(r => r.Temperature ?? 0);
                    var averageUsage = records.Average(r => r.OverallUsage ?? 0);
                    var averageMemoryUsage = records.Average(r => r.MemoryUsage ?? 0);

                    MessageBox.Show($"Průměrné hodnoty pro zařízení {selectedDevice.Name}:\n" +
                                    $"Teplota: {averageTemperature} °C\n" +
                                    $"Celkové využití: {averageUsage}%\n" +
                                    $"Využití paměti: {averageMemoryUsage}%",
                        "Průměrné informace", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Žádné záznamy pro vybrané zařízení.", "Chyba", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Musíte vybrat počítač.", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
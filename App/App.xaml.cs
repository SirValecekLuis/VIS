using System;
using System.Windows;
using Vis.App;

namespace VIS.App
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            LoginWindow mainWindow = new LoginWindow();

            mainWindow.Title = "VIS JAN0895";

            mainWindow.Show();
        }

        [STAThread]
        public static void Main(string[] args)
        {
            var db = DatabaseManager.Instance;

            db.CreateDatabase();
            db.ExportClassAttributesToCsv(true,
                typeof(AnalysisReport),
                typeof(Device),
                typeof(Disk),
                typeof(GraphicsCard),
                typeof(Motherboard),
                typeof(PredefinedOperation),
                typeof(Processor),
                typeof(Record),
                typeof(User),
                typeof(UserOperation)
            );

            var user = new User(1, "admin", "admin", "admin");
            db.AddCSVRecord(user);
            db.InsertRecord("User", user);

            for (int i = 0; i < 100; i++)
            {
                var moth = new Motherboard(db.GetNextId("Motherboard"), $"Motherboard no {i}.",
                    $"Manufacturer no {i}.");
                db.InsertRecord("Motherboard", moth);
            }

            for (int i = 0; i < 100; i++)
            {
                var device = new Device(db.GetNextId("Device"), $"Computer no {i}.", $"EB{i * 10 + i}", i);

                db.InsertRecord("Device", device);
            }

            for (int i = 0; i < 100; i++)
            {
                var cpu = new Processor(db.GetNextId("Processor"), $"Processor no {i}.", $"Manufacturer no {i}.", i);

                db.InsertRecord("Processor", cpu);
            }

            var random = new Random();
            for (int i = 0; i < 1000; i++)
            {
                var record = new Record(db.GetNextId("Record"), "CPU temp", random.Next(1, 100), null,
                    random.Next(1, 100), null, DateTime.Now, (float) (random.NextDouble() * 100), (float) (random.NextDouble() * 100),
                    null);

                db.InsertRecord("Record", record);
            }

            App app = new App();
            app.Run();
        }
    }
}
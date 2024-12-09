namespace VIS;

public class Start
{
    public static void Main()
    {
        var dbManager = DatabaseManager.Instance;
        var connection = dbManager.GetSQLiteConnection();
        
        dbManager.ExportClassAttributesToCsv(false, 
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

        var user = new User(dbManager.GetNextId("User"), "hihi", "heslo", "admin");
        var ret = dbManager.GetAllOrInsertSQLRecord("User", user);

        foreach (var returnObj in ret)
        {
            Console.WriteLine($"{returnObj.Id}");
        }
        
        dbManager.AddCSVRecord(user);
        var retCsv = dbManager.GetCSVRecords<User>();

        foreach (var variable in retCsv)
        {
            Console.WriteLine($"{variable.Id}");
        }
    }
}
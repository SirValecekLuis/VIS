using System;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using System.Reflection;
using Dapper;
using CsvHelper;
using CsvHelper.Configuration;

public sealed class DatabaseManager
{
    private static readonly Lazy<DatabaseManager> _instance =
        new Lazy<DatabaseManager>(() => new DatabaseManager());

    private string _dbPath;
    private string _csvPath;

    private static string GetDbPath(string FileName)
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        string[] path = assembly.Location.Split('\\');
        return String.Join('\\', path.Take(path.Length - 4).ToArray()) + "\\" + FileName;
    }

    private DatabaseManager()
    {
        _dbPath = GetDbPath("Database\\local_database.db");
        _csvPath = GetDbPath("Database\\");
    }


    public static DatabaseManager Instance => _instance.Value;

    public SQLiteConnection GetSQLiteConnection()
    {
        if (!File.Exists(_dbPath))
        {
            CreateDatabase();
        }

        var conn = new SQLiteConnection($"Data Source={_dbPath};Version=3;");
        conn.Open();

        return conn;
    }

    public void CreateDatabase()
    {
        if (File.Exists(_dbPath))
        {
            File.Delete(_dbPath);
        }

        SQLiteConnection.CreateFile(_dbPath);

        using var connection = new SQLiteConnection($"Data Source={_dbPath};Version=3;");
        connection.Open();

        const string placeholderSql =
            "-- Tabulka Motherboard\nCREATE TABLE Motherboard (\n    Id INTEGER PRIMARY KEY,\n    Name TEXT NOT NULL,\n    Manufacturer TEXT NOT NULL\n);\n\n-- Tabulka Processor\nCREATE TABLE Processor (\n    Id INTEGER PRIMARY KEY,\n    Name TEXT NOT NULL,\n    Manufacturer TEXT NOT NULL,\n    MotherboardId INTEGER,\n    FOREIGN KEY (MotherboardId) REFERENCES Motherboard(Id)\n);\n\n-- Tabulka GraphicsCard\nCREATE TABLE GraphicsCard (\n    Id INTEGER PRIMARY KEY,\n    Name TEXT NOT NULL,\n    Manufacturer TEXT NOT NULL,\n    MotherboardId INTEGER,\n    FOREIGN KEY (MotherboardId) REFERENCES Motherboard(Id)\n);\n\n-- Tabulka Disk\nCREATE TABLE Disk (\n    Id INTEGER PRIMARY KEY,\n    Name TEXT NOT NULL,\n    Manufacturer TEXT NOT NULL,\n    MotherboardId INTEGER,\n    FOREIGN KEY (MotherboardId) REFERENCES Motherboard(Id)\n);\n\n-- Tabulka Device\nCREATE TABLE Device (\n    Id INTEGER PRIMARY KEY,\n    Name TEXT NOT NULL,\n    Location TEXT,\n    MotherboardId INTEGER,\n    FOREIGN KEY (MotherboardId) REFERENCES Motherboard(Id)\n);\n\n-- Tabulka User\nCREATE TABLE User (\n    Id INTEGER PRIMARY KEY,\n    Name TEXT NOT NULL UNIQUE,\n    Password TEXT NOT NULL,\n    Role TEXT NOT NULL\n);\n\n-- Tabulka PredefinedOperation\nCREATE TABLE PredefinedOperation (\n    Id INTEGER PRIMARY KEY,\n    Name TEXT NOT NULL,\n    Description TEXT,\n    Command TEXT NOT NULL\n);\n\n-- Tabulka UserOperation\nCREATE TABLE UserOperation (\n    Id INTEGER PRIMARY KEY,\n    UserId INTEGER NOT NULL,\n    DeviceId INTEGER NOT NULL,\n    Command TEXT NOT NULL,\n    ExecTime DATETIME NOT NULL,\n    FOREIGN KEY (UserId) REFERENCES User(Id),\n    FOREIGN KEY (DeviceId) REFERENCES Device(Id)\n);\n\n-- Tabulka AnalysisReport\nCREATE TABLE AnalysisReport (\n    Id INTEGER PRIMARY KEY,\n    DeviceId INTEGER NOT NULL,\n    Date DATETIME NOT NULL,\n    Data TEXT,\n    FOREIGN KEY (DeviceId) REFERENCES Device(Id)\n);\n\n-- Tabulka Record\nCREATE TABLE Record (\n    Id INTEGER PRIMARY KEY,\n    Type TEXT NOT NULL,\n    DeviceId INTEGER NOT NULL,\n    DiskId INTEGER,\n    CpuId INTEGER,\n    GpuId INTEGER,\n    Date DATETIME NOT NULL,\n    Temperature REAL,\n    OverallUsage REAL,\n    MemoryUsage REAL,\n    FOREIGN KEY (DeviceId) REFERENCES Device(Id),\n    FOREIGN KEY (DiskId) REFERENCES Disk(Id),\n    FOREIGN KEY (CpuId) REFERENCES Processor(Id),\n    FOREIGN KEY (GpuId) REFERENCES GraphicsCard(Id)\n);";
        if (!string.IsNullOrWhiteSpace(placeholderSql))
        {
            using var command = new SQLiteCommand(placeholderSql, connection);
            command.ExecuteNonQuery();
        }

        connection.Close();
    }

    public T? ExecuteQuery<T>(string query, params SQLiteParameter[] parameters) where T : class
    {
        using var connection = GetSQLiteConnection();
        using var command = new SQLiteCommand(query, connection);

        foreach (var param in parameters)
        {
            command.Parameters.Add(param);
        }

        using var reader = command.ExecuteReader();

        if (!reader.HasRows)
            return null;

        var properties = typeof(T).GetProperties();
        if (reader.Read())
        {
            var obj = Activator.CreateInstance<T>();

            foreach (var property in properties)
            {
                if (!reader.IsDBNull(reader.GetOrdinal(property.Name)))
                {
                    var value = reader[property.Name];
                    property.SetValue(obj, value);
                }
            }

            return obj;
        }

        return null;
    }

    public List<T> GetAllRecords<T>(string tableName)
    {
        using var connection = GetSQLiteConnection();
        string selectQuery = $"SELECT * FROM {tableName}";
    
        return connection.Query<T>(selectQuery)?.ToList() ?? new List<T>();
    }

    
    public void InsertRecord<T>(string tableName, T record) where T: class
    {
        using var connection = GetSQLiteConnection();
        var properties = typeof(T).GetProperties();
        var columnNames = string.Join(", ", properties.Select(p => p.Name));
        var parameterNames = string.Join(", ", properties.Select(p => "@" + p.Name));

        string insertQuery = $"INSERT INTO {tableName} ({columnNames}) VALUES ({parameterNames})";
        connection.Execute(insertQuery, record);
        
        AddCSVRecord(record);
    }

    public int GetNextId(string tableName)
    {
        using var connection = GetSQLiteConnection();

        string query = $"SELECT COALESCE(MAX(Id), 0) FROM {tableName}";

        int maxId = connection.ExecuteScalar<int>(query);

        return maxId + 1;
    }

    public void ExportClassAttributesToCsv(bool overwrite = false, params Type[] classes)
    {
        Directory.CreateDirectory(_csvPath);

        foreach (var classType in classes)
        {
            var properties = classType.GetProperties(
                BindingFlags.Public |
                BindingFlags.Instance |
                BindingFlags.DeclaredOnly
            );

            string csvFileName = Path.Combine(_csvPath, $"{classType.Name}.csv");

            if (File.Exists(csvFileName) && !overwrite)
            {
                continue;
            }

            var header = string.Join(",", properties.Select(p => p.Name));
            header += "\n";

            File.WriteAllText(csvFileName, header);

            Console.WriteLine($"Vytvořen soubor: {csvFileName}");
        }
    }

    public void AddCSVRecord<T>(T record) where T : class
    {
        string className = typeof(T).Name;
        string csvPath = Path.Combine(_csvPath, $"{className}.csv");

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = !File.Exists(csvPath),
            PrepareHeaderForMatch = args => args.Header.ToUpper()
        };

        using (var stream = new FileStream(csvPath, FileMode.Append, FileAccess.Write, FileShare.Read))
        using (var writer = new StreamWriter(stream))
        using (var csv = new CsvWriter(writer, config))
        {
            csv.WriteRecord(record);
            csv.NextRecord();
        }
    }

    public List<T> GetCSVRecords<T>() where T : class
    {
        string className = typeof(T).Name;
        string csvPath = Path.Combine(_csvPath, $"{className}.csv");

        if (!File.Exists(csvPath))
        {
            return new List<T>();
        }

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            PrepareHeaderForMatch = args => args.Header.ToUpper()
        };

        using (var reader = new StreamReader(csvPath))
        using (var csv = new CsvReader(reader, config))
        {
            return csv.GetRecords<T>().ToList();
        }
    }

    private void UpdateCSVRecord<T>(T updatedRecord) where T : class
    {
        string className = typeof(T).Name;
        string csvPath = Path.Combine(_csvPath, $"{className}.csv");

        if (!File.Exists(csvPath))
            return;

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            PrepareHeaderForMatch = args => args.Header.ToUpper()
        };

        var records = GetCSVRecords<T>().ToList();

        var idProperty = typeof(T).GetProperty("Id");
        if (idProperty == null)
            throw new InvalidOperationException("Třída nemá vlastnost 'Id'.");

        var idValue = idProperty.GetValue(updatedRecord);
        var recordToUpdate = records.FirstOrDefault(r => idProperty.GetValue(r).Equals(idValue));
        if (recordToUpdate != null)
        {
            records.Remove(recordToUpdate);
            records.Add(updatedRecord);

            using var writer = new StreamWriter(csvPath);
            using var csv = new CsvWriter(writer, config);
            csv.WriteRecords(records);
        }
    }

    public void UpdateRecord<T>(string tableName, T updatedRecord) where T : class
    {
        using var connection = GetSQLiteConnection();

        var properties = updatedRecord.GetType().GetProperties();

        var idProperty = properties.FirstOrDefault(p => p.Name == "Id");
        if (idProperty == null)
            throw new InvalidOperationException("Objekt nemá atribut 'Id'.");

        var idValue = idProperty.GetValue(updatedRecord);
        if (idValue == null)
            throw new InvalidOperationException("Hodnota atributu 'Id' je null.");

        var columnUpdates =
            string.Join(", ", properties.Where(p => p.Name != "Id").Select(p => $"{p.Name} = @{p.Name}"));
        string updateQuery = $"UPDATE {tableName} SET {columnUpdates} WHERE Id = @Id";

        var parameters = properties.ToDictionary(p => p.Name, p => p.GetValue(updatedRecord));
        parameters["Id"] = idValue;

        connection.Execute(updateQuery, parameters);

        UpdateCSVRecord(updatedRecord);
    }
}
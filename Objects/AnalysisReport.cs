namespace VIS;

public class AnalysisReport
{
    public long Id { get; set; }

    public long DeviceId { get; set; }

    public DateTime Date { get; set; }
    
    public string Data { get; set; }

    public AnalysisReport(long id, long devid, DateTime date, string data)
    {
        Id = id;
        DeviceId = devid;
        Date = date;
        Data = data;
    }
}
namespace VIS;

public class Record
{
    public long Id { get; set; }
    
    public string Type { get; set; }
    
    public long DeviceId { get; set; }
    
    public long? DiskId { get; set; }
    
    public long? CpuId { get; set; }
    
    public long? GpuId { get; set; }
    
    public DateTime Date { get; set; }
    
    public float? Temperature { get; set; }
    
    public float? OverallUsage { get; set; }
    
    public float? MemoryUsage { get; set; }

    public Record(long id, string type, long devid, long? diskid, long? cpuid, long? gpuid, DateTime date, float? temp,
        float? usage, float? memory)
    {
        Id = id;
        Type = type;
        DeviceId = devid;
        DiskId = diskid;
        CpuId = cpuid;
        GpuId = gpuid;
        Date = date;
        Temperature = temp;
        OverallUsage = usage;
        MemoryUsage = memory;
    }

    public Record()
    {
        
    }
}
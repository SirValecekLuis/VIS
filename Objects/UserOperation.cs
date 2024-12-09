namespace VIS;

public class UserOperation
{
    public long Id { get; set; }
    
    public long UserId { get; set; }
    
    public long DeviceId { get; set; }
    
    public DateTime ExecTime { get; set; }

    public UserOperation(long id, long userid, long deviceid, DateTime exec)
    {
        Id = id;
        UserId = userid;
        DeviceId = deviceid;
        ExecTime = exec;
    }
}
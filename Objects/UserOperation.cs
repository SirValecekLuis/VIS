namespace VIS;

public class UserOperation
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public long DeviceId { get; set; }

    public string Command { get; set; }

    public DateTime ExecTime { get; set; }

    public UserOperation(long id, long userid, long deviceid, string command, DateTime exectime)
    {
        Id = id;
        UserId = userid;
        DeviceId = deviceid;
        Command = command;
        ExecTime = exectime;
    }

    public UserOperation()
    {
    }
}
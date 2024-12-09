namespace VIS;

public class PredefinedOperation
{
    public long Id { get; set; }
    
    public string Name { get; set; }
    
    public string Description { get; set; }
    
    public string Command { get; set; }

    public PredefinedOperation(long id, string name, string desc, string comm)
    {
        Id = id;
        Name = name;
        Description = desc;
        Command = comm;
    }
}
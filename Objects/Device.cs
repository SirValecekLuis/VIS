namespace VIS;

public class Device
{
    public long Id { get; set; }

    public string Name { get; set; }

    public string Location { get; set; }

    public int MotherboardId { get; set; }

    public Device(long id, string name, string location, int motherboardid)
    {
        Id = id;
        Name = name;
        Location = location;
        MotherboardId = motherboardid;
    }

    public Device()
    {
        
    }
}
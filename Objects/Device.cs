namespace VIS;

public class Device
{
    public long Id { get; set; }

    public string Name { get; set; }

    public string Location { get; set; }

    public int MotherboardId { get; set; }

    public Device(long id, string name, string loc, int mbid)
    {
        Id = id;
        Name = name;
        Location = loc;
        MotherboardId = mbid;
    }
}
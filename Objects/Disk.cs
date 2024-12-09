namespace VIS;

public class Disk
{
    public long Id { get; set; }

    public string Name { get; set; }

    public string Manufacturer { get; set; }

    public long MotherboardId { get; set; }

    public Disk(long id, string name, string manufacturer, long motherboardid)
    {
        Id = id;
        Name = name;
        Manufacturer = manufacturer;
        MotherboardId = motherboardid;
    }
}
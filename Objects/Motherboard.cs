namespace VIS;

public class Motherboard
{
    public long Id { get; set; }
    
    public string Name { get; set; }

    public string Manufacturer { get; set; }

    public Motherboard(long id, string name, string manufacturer)
    {
        Id = id;
        Name = name;
        Manufacturer = manufacturer;
    }
}
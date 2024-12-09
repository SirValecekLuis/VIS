namespace VIS;

public class User
{
    public long Id { get; set; }

    public string Name { get; set; }

    public string Password { get; set; }

    public string Role { get; set; }

    public User(long id, string name, string password, string role)
    {
        Id = id;
        Name = name;
        Password = password;
        Role = role;
    }

    public User()
    {
        
    }
}
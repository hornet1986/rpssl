namespace Rpssl.Domain.Entities;
public class Choice
{
    // EF Core will treat 'Id' as the key by convention
    public int Id { get; private set; }
    public string Name { get; private set; } = null!;

    private Choice() { } // EF
    public Choice(int id, string name)
    {
        Id = id;
        Name = name;
    }
}

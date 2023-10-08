namespace CharaTrack;

/// <summary>
/// A character object.
/// </summary>
public class Character
{
    public string name { get; set; }
    public string id { get; }
    public string owner { get; set; }
    public string? origin { get; set; }
    public string? bio { get; set; }
    public string? description { get; set; }
    public Inventory inventory { get; }
    public Inventory knowledge { get; }
    public Stack<string> locationHistory { get; }

    /// <summary>
    /// A method that gets the relationships of the character
    /// </summary>
    /// <param name="lookup"></param>
    /// <returns></returns>
    public HashSet<Relations> relationships(Func<string, HashSet<Relations>> lookup)
    {
        return lookup(id);
    }

    public Character(string name, string owner)
    {
        this.name = name;
        this.owner = owner;
        this.id = owner + "." + name + "." + System.DateTime.UtcNow.ToString("ddMMyyyy.hhmm");
        this.inventory = new Inventory();
        this.knowledge = new Inventory();
        this.locationHistory = new Stack<string>();
    }

    public override int GetHashCode()
    {
        return id.GetHashCode();
    }
}
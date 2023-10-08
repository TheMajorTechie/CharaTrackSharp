namespace CharaTrack;

/// <summary>
/// A character object.
/// </summary>
public class Character
{
    public string name { get; set; }
    public int id { get; }
    public string owner { get; set; }
    public string? origin { get; set; }
    public string? bio { get; set; }
    public string? description { get; set; }
    public Inventory inventory { get; }
    public Inventory knowledge { get; }
    public Stack<string> locationHistory { get; }

    public HashSet<Relations> relationships(Func<string, Relations> lookup)
    {

    }

    public Character()
    {

    }

}
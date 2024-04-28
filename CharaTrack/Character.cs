namespace CharaTrack;

/// <summary>
/// A Character object.
/// </summary>
public class Character
{
    static string[] AllowedOptionalCharacteristics = { "characterBio", "characterGender", "characterAgeInYears",
        "characterHeightInMeters", "characterHeightInFeet", "characterWeightInKilos", "characterWeightInPounds", "characterSpecies",
        "characterRace", "characterEthnicity", "characterMiscNotes"};
    public Dictionary<string, string> Characteristics { get; }

    //Base character characteristics
    public string characterName;
    public string characterMaker;
    public string characterRevision;

    //internal character characteristics
    public string characterID;
    public string characterCreationDate;
    public string characterLastModifiedDate;

    /// <summary>
    /// Default character creator
    /// </summary>
    /// <param name="name"></param>
    /// <param name="maker"></param>
    /// <param name="revision"></param>
    public Character(string name, string maker, string revision)
    {
        Characteristics = new Dictionary<string, string>();

        this.characterName = name;
        this.characterMaker = maker;
        this.characterRevision = revision;
        this.characterCreationDate = System.DateTime.UtcNow.ToString("ddMMyyyy.hhmm");
        this.characterLastModifiedDate = characterCreationDate;
        this.characterID = maker + "." + name + "." + revision;

        Characteristics.Add("name", name);
        Characteristics.Add("maker", maker);
        Characteristics.Add("revision", revision);
    }

    /// <summary>
    /// Hashcode Override
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        return characterID.GetHashCode();
    }

    public string? GetCharacteristic(string type)
    {
        switch(type)
        {
            //cases that are guaranteed to exist
            case "name": return characterName;
            case "maker": return characterMaker;
            case "revision": return characterRevision;
            case "id": return characterID;
            case "creationDate": return characterCreationDate;
            case "lastModifiedDate": return characterLastModifiedDate;

            //outside of "base" characteristics, check the dictionary
            default:
                {
                    if(Characteristics.ContainsKey(type))
                    {
                        return Characteristics[type];
                    }
                    return null;
                }
                
        }
    }

    /// <summary>
    /// Update the properties of the character. For "base" characteristics
    /// that the Character ID relies on, update the character ID as well.
    /// </summary>
    /// <param name="type"></param> Specify which characteristic to update
    /// <param name="contents"></param> What to update the characteristic with
    public void UpdateCharacteristic(string type, string contents)
    {
        switch(type)
        {
            case "name":
                {
                    this.characterName = contents;
                    Characteristics["name"] = contents;
                    this.characterID = characterMaker + "." + characterName + "." + characterRevision;
                    break;
                }
            case "maker":
                {
                    this.characterMaker = contents;
                    Characteristics["maker"] = contents;
                    this.characterID = characterMaker + "." + characterName + "." + characterRevision;
                    break;
                }
            case "revision":
                {
                    this.characterRevision = contents;
                    Characteristics["revision"] = contents;
                    this.characterID = characterMaker + "." + characterName + "." + characterRevision;
                    break;
                }

            default: UpdateMiscCharacteristics(type, contents); break;
        }
        this.characterLastModifiedDate = System.DateTime.UtcNow.ToString("ddMMyyyy.hhmm");
    }

    /// <summary>
    /// Updates miscellaneous characteristics. If they have not yet been made, then make them.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="contents"></param>
    private void UpdateMiscCharacteristics(string type, string contents)
    {
        if(AllowedOptionalCharacteristics.Contains(type))
        {
            if(Characteristics.ContainsKey(type)) {
                Characteristics.Remove(type);
            }
            Characteristics.Add(type, contents);
        }
    }
}
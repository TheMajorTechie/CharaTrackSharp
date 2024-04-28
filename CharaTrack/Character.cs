namespace CharaTrack;

/// <summary>
/// A Character object.
/// </summary>
public class Character
{
    //Base character characteristics
    public string characterName;
    public string characterMaker;
    public string characterRevision;
    public string characterID;
    public string characterCreationDate;
    public string characterLastModifiedDate;

    //optional characteristics
    public string? characterBio;
    public string? characterGender;
    public string? characterAgeInYears;
    public string? characterHeightInMeters;
    public string? characterWeightInKilos;
    public string? characterHeightInFeet;
    public string? characterWeightInPounds;
    public string? characterSpecies;
    public string? characterRace;
    public string? characterEthnicity;

    public string? characterMiscNotes;

    /// <summary>
    /// Default character creator
    /// </summary>
    /// <param name="name"></param>
    /// <param name="maker"></param>
    /// <param name="revision"></param>
    public Character(string name, string maker, string revision)
    {
        this.characterName = name;
        this.characterMaker = maker;
        this.characterRevision = revision;
        this.characterCreationDate = System.DateTime.UtcNow.ToString("ddMMyyyy.hhmm");
        this.characterLastModifiedDate = characterCreationDate;
        this.characterID = maker + "." + name + "." + revision;
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

            // TODO: ADD SUPPORT FOR OPTIONAL CHARACTERISTICS
            default: return null;
        }
    }

    public void UpdateCharacteristic(string type, string contents)
    {
        switch(type)
        {
            case "name":
                {
                    this.characterName = contents;
                    this.characterID = characterMaker + "." + characterName + "." + characterRevision;
                    break;
                }
            case "maker":
                {
                    this.characterMaker = contents;
                    this.characterID = characterMaker + "." + characterName + "." + characterRevision;
                    break;
                }
            case "revision":
                {
                    this.characterRevision = contents;
                    this.characterID = characterMaker + "." + characterName + "." + characterRevision;
                    break;
                }
            default: break;
        }
        this.characterLastModifiedDate = System.DateTime.UtcNow.ToString("ddMMyyyy.hhmm");
    }
}
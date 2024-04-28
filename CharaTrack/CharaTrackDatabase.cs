using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CharaTrack;

public class CharaTrackDatabase : AbstractDatabase
{
    /// <summary>
    /// Stores all characters in a key-value pair of character ID and character
    /// </summary>
    [JsonInclude]
    public Dictionary<string, Character> Characters { get; }

    public CharaTrackDatabase()
    {
        //This dictionary holds characters using characterID keys.
        //The format of these IDs is Maker.Name.Revision
        Characters = new Dictionary<string, Character>();
        
        //CharacterIDAliases = new Dictionary<string, string>();
    }

    public int getDatabaseSize()
    {
        return Characters.Count;
    }

    public override void Save(string filename)
    {
        throw new NotImplementedException();
    }

    public override void AddCharacter(string name, string maker, string revision)
    {
        string tempID = maker + "." + name + "." + revision;
        if (Characters.ContainsKey(tempID))
        {
            System.Diagnostics.Debug.WriteLine("A character of this revision already exists!");
            throw new ArgumentException("A character of this revision already exists!");
        }

        else
        {
            Characters[tempID] = new Character(name, maker, revision);  
        }
    }

    public override void RemoveCharacter(string characterID)
    {
        if(!Characters.Remove(characterID))
        {
            throw new CharacterNotFoundException();
        }
    }

    public override string GetCharacterContents(string characterID, string contentType)
    {
        ThrowIfCharacterAbsent(characterID);
        string? characteristic = Characters[characterID].GetCharacteristic(contentType);
        if (characteristic != null)
        {
            return characteristic;
        }
        else
        {
            throw new CharacteristicNullException();
        }
    }

    public override IList<string> SetCharacterContents(string characterID, string paramType, string contents)
    {
        ThrowIfCharacterAbsent(characterID);
        switch(paramType)
        {
            case "name":
            case "maker":
            case "revision":
                {
                    Character tempCharacter = Characters[characterID];              //temporarily store the character in memory for updating
                    tempCharacter.UpdateCharacteristic(paramType, contents);
                    Characters.Add(tempCharacter.characterID, tempCharacter);       //update dictionary with new character ID and remove old one
                    Characters.Remove(characterID);
                    break;
                }
        }

        // TODO: IMPLEMENT CHARACTER RELATIONSHIP GRAPH
        return new List<string>();
    }

    public void OrphanCharacter(string characterID)
    {
        ThrowIfCharacterAbsent(characterID);
        SetCharacterContents(characterID, "maker", "CharaTrack");
    }

    public void AdoptCharacter(string characterID, string adopter)
    {
        ThrowIfCharacterAbsent(characterID);
        if (Characters[characterID].characterMaker == "CharaTrack")
        {
            SetCharacterContents(characterID, "maker", adopter);
        }
    }

    public override IEnumerable<int> GetDirectCharacterRelations(string characterID)
    {
        throw new NotImplementedException();
    }

    public override void AddCharacterRelation(string characterID, int relationID, string relationType)
    {
        throw new NotImplementedException();
    }

    public void ThrowIfCharacterAbsent(string characterID)
    {
        if (!CheckIfCharacterPresent(characterID))
            throw new CharacterNotFoundException();
    }

    public bool CheckIfCharacterPresent(string characterID)
    {
        return Characters.ContainsKey(characterID);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CharaTrack;

public class CharacterCreationException : Exception { }

public class InvalidNameException : Exception { }

public class CharacterNotFoundException : Exception { }

public class DatabaseReadWriteException : Exception { }

public class CharacteristicNullException : Exception { }

public abstract class AbstractDatabase
{
    /// <summary>
    /// True if the database has been modified since it was created or last saved.
    /// False if otherwise.
    /// </summary>
    [JsonIgnore]
    public bool Modified { get; protected set; }

    public AbstractDatabase() { }

    /// <summary>
    /// Writes the contents of the database to the specified file using JSON.
    /// </summary>
    /// <param name="filename"></param>
    public abstract void Save(string filename);

    /// <summary>
    /// Add a character to the database
    /// </summary>
    /// <param name="name"></param>
    /// <param name="maker"></param>
    /// <param name="revision"></param>
    public abstract void AddCharacter(string name, string maker, string revision);

    /// <summary>
    /// Remove a character from the database
    /// </summary>
    /// <param name="characterID"></param>
    public abstract void RemoveCharacter(string characterID);

    /// <summary>
    /// Retrieves the contents of a character as specified by their ID.
    /// </summary>
    /// <param name="characterID"></param>
    /// <returns></returns>
    public abstract object GetCharacterContents(string characterID, string contentType);

    /// <summary>
    /// If the ID doesn't exist, throw an InvalidNameException.
    /// 
    /// Otherwise, attempt to update the specified parameter.
    /// 
    /// If an exception is not thrown, then return a list of the character ID
    /// plus the IDs of all characters that may be affected by the update.
    /// 
    /// This will require the creation of a dependency/relationship graph object.
    /// </summary>
    /// <param name="characterID"></param>
    /// <param name="contents"></param>
    /// <returns></returns>
    public abstract IList<string> SetCharacterContents(string characterID, string paramType, string contents);

    /// <summary>
    /// Get a list of character IDs that are directly related (i.e., not a "friend of a friend") to a specified
    /// character.
    /// 
    /// "Directly related" in this case means any of the following:
    /// - friends
    /// - acquaintences
    /// - partners
    /// - siblings
    /// - parents
    /// - offspring
    /// - pets
    /// </summary>
    /// <param name="characterID"></param>
    /// <returns></returns>
    public abstract IEnumerable<int> GetDirectCharacterRelations(string characterID);

    /// <summary>
    /// Add two characters to a relationship. This should update the relationship graph!
    /// </summary>
    /// <param name="characterID"></param>
    /// <param name="relationID"></param>
    public abstract void AddCharacterRelation(string characterID, int relationID, string relationType);
}
namespace CharaTrack;


[TestClass]
public class CharacterTests
{
    [TestMethod]
    public void CharacterConstructorTest()
    {
        Character c = new Character("Zoey", "Tonkus", "0");
        Console.WriteLine(c.GetHashCode().ToString());
        Assert.AreEqual("Tonkus.Zoey.0", c.characterID);
    }

    [TestMethod]
    public void CharactersEqualRevisionTest()
    {
        Character c = new Character("Zoey", "Tonkus", "0");
        Character d = new Character("Zoey", "Tonkus", "0");
        Assert.AreEqual(c.GetHashCode(), d.GetHashCode());
    }

    [TestMethod]
    public void CharactersNotEqualRevisionTest()
    {
        Character c = new Character("Zoey", "Tonkus", "0");
        Character d = new Character("Zoey", "Tonkus", "1");
        Assert.AreNotEqual(c.GetHashCode(), d.GetHashCode());
    }

    [TestMethod]
    public void CharactersDateCreatedTest()
    {
        Character c = new Character("Zoey", "Tonkus", "0");
        Assert.AreEqual(System.DateTime.UtcNow.ToString("ddMMyyyy.hhmm"), c.characterCreationDate.ToString());
    }


    //tests involving the database---------------------------------------
    [TestMethod]
    public void CharactersDatabaseCreateTest()
    {
        CharaTrackDatabase db = new CharaTrackDatabase();
        db.AddCharacter("Zoey", "Tonkus", "0");
        Assert.IsTrue(db.getDatabaseSize() > 0);
        Assert.AreEqual("Zoey", db.GetCharacterContents("Tonkus.Zoey.0", "name"));
        Assert.AreEqual("Tonkus", db.GetCharacterContents("Tonkus.Zoey.0", "maker"));
        Assert.AreEqual("0", db.GetCharacterContents("Tonkus.Zoey.0", "revision"));
    }

    [TestMethod]
    public void CharactersDatabaseDuplicateRevisionsTest()
    {
        CharaTrackDatabase db = new CharaTrackDatabase();
        db.AddCharacter("Zoey", "Tonkus", "0");
        Assert.ThrowsException<ArgumentException>(() => db.AddCharacter("Zoey", "Tonkus", "0"));
    }

    [TestMethod]
    public void RemoveCharacterTest()
    {
        CharaTrackDatabase db = new CharaTrackDatabase();
        db.AddCharacter("Zoey", "Tonkus", "0");
        Assert.IsTrue(db.getDatabaseSize() == 1);
        db.RemoveCharacter("Tonkus.Zoey.0");
        System.Diagnostics.Debug.WriteLine(db.getDatabaseSize());
        Assert.IsTrue(db.getDatabaseSize() == 0);
    }

    [TestMethod]
    public void CharacterRenameTest()
    {
        CharaTrackDatabase db = new CharaTrackDatabase();
        db.AddCharacter("Zoey", "Tonkus", "0");
        db.SetCharacterContents("Tonkus.Zoey.0", "name", "Zo");
        Assert.AreEqual("Zo", db.GetCharacterContents("Tonkus.Zo.0", "name"));
        Assert.IsTrue(db.getDatabaseSize() == 1);
    }

    [TestMethod]
    public void CharacterDoesNotExistRemoveTest()
    {
        CharaTrackDatabase db = new CharaTrackDatabase();
        Assert.ThrowsException<CharacterNotFoundException>(() => db.RemoveCharacter("Tonkus.Zoey.0"));
    }

    [TestMethod]
    public void CharacterDoesNotExistEditTest()
    {
        CharaTrackDatabase db = new CharaTrackDatabase();
        Assert.ThrowsException<CharacterNotFoundException>(() => db.SetCharacterContents("Tonkus.Zoey.0", "name", "asdf"));
    }

    [TestMethod]
    public void GetUnknownCharacteristicTest()
    {
        CharaTrackDatabase db = new CharaTrackDatabase();
        db.AddCharacter("Zoey", "Tonkus", "0");
        Assert.ThrowsException<CharacteristicNullException>(() => db.GetCharacterContents("Tonkus.Zoey.0", "age"));
        Assert.ThrowsException<CharacteristicNullException>(() => db.GetCharacterContents("Tonkus.Zoey.0", "blah"));
    }
}

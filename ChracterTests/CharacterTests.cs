namespace CharaTrack;


[TestClass]
public class CharacterTests
{
    [TestMethod]
    public void CharacterConstructorTest()
    {
        Character c = new Character("Zoey", "Tonkus");
        Console.WriteLine(c.GetHashCode().ToString());
        Assert.AreEqual("Tonkus.Zoey." + System.DateTime.UtcNow.ToString("ddMMyyyy.hhmm"), c.id);
    }

}

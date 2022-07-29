using XAT.Game.Formats.Pap;
using XAT.Game.Formats.Sklb;

namespace XAT.Test.Game;

[TestClass]
public class GameBasicTests
{
    [TestMethod]
    public void ReadSklb()
    {
        var result = SklbFormat.FromFile("./TestFiles/skeleton.sklb");
        Assert.AreNotEqual(0, result.HavokData.Length); 
    }

    [TestMethod]
    public void ReadWriteReadSklb()
    {
        var result = SklbFormat.FromFile("./TestFiles/skeleton.sklb");
        Assert.AreNotEqual(0, result.HavokData.Length);

        string tmpFile = Path.GetTempFileName();
        result.ToFile(tmpFile);

        result = SklbFormat.FromFile(tmpFile);
        Assert.AreNotEqual(0, result.HavokData.Length);

        File.Delete(tmpFile);
    }

    [TestMethod]
    public void ReadPap()
    {
        var result = PapFormat.FromFile("./TestFiles/animation.pap");
        Assert.AreEqual(101, result.Skeleton);
        Assert.AreEqual(1, result.Animations.Count);
    }

    [TestMethod]
    public void ReadWriteReadPap()
    {
        var result = PapFormat.FromFile("./TestFiles/animation.pap");
        Assert.AreEqual(101, result.Skeleton);

        result.Skeleton = 42;

        string tmpFile = Path.GetTempFileName();
        result.ToFile(tmpFile);

        result = PapFormat.FromFile(tmpFile);
        Assert.AreEqual(42, result.Skeleton);

        File.Delete(tmpFile);
    }
}
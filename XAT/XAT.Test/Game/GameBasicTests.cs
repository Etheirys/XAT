using XAT.Common.FFXIV.Files;

namespace XAT.Test.Game;

[TestClass]
public class GameBasicTests
{
    [TestMethod]
    public void ReadSklb()
    {
        var result = Sklb.FromFile("./TestFiles/skeleton.sklb");
        Assert.AreNotEqual(0, result.HavokData.Length); 
    }

    [TestMethod]
    public void ReadWriteReadSklb()
    {
        var result = Sklb.FromFile("./TestFiles/skeleton.sklb");
        Assert.AreNotEqual(0, result.HavokData.Length);

        string tmpFile = Path.GetTempFileName();
        result.ToFile(tmpFile);

        result = Sklb.FromFile(tmpFile);
        Assert.AreNotEqual(0, result.HavokData.Length);

        File.Delete(tmpFile);
    }

    [TestMethod]
    public void ReadPap()
    {
        var result = Pap.FromFile("./TestFiles/animation.pap");
        Assert.AreEqual(101, result.Skeleton);
        Assert.AreEqual(1, result.AnimInfos.Count);
    }

    [TestMethod]
    public void ReadWriteReadPap()
    {
        var result = Pap.FromFile("./TestFiles/animation.pap");
        Assert.AreEqual(101, result.Skeleton);

        result.Skeleton = 42;

        string tmpFile = Path.GetTempFileName();
        result.ToFile(tmpFile);

        result = Pap.FromFile(tmpFile);
        Assert.AreEqual(42, result.Skeleton);

        File.Delete(tmpFile);
    }
}
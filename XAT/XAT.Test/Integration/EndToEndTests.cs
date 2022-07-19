using XAT.Common.FFXIV.Files;
using XAT.Common.Havok;

namespace XAT.Test.Integration;

[TestClass]
public class EndToEndTests
{
    [TestMethod]
    public void SklbToFbx()
    {
        var sklb = Sklb.FromFile("./TestFiles/skeleton.sklb");
        Assert.AreNotEqual(0, sklb.HavokData.Length);

        string tmpFileName = Path.GetTempFileName();
        RawHavokInterop.CreateContainer(tmpFileName).Wait();

        string sklbHavok = Path.GetTempFileName();
        File.WriteAllBytes(sklbHavok, sklb.HavokData);
        RawHavokInterop.AddSkeleton(tmpFileName, sklbHavok, 0, tmpFileName).Wait();

        var fbxPortResults = RawHavokInterop.ToFbxSkeleton(tmpFileName, 0, tmpFileName).Result;
        Assert.AreEqual(105, fbxPortResults);

        var resultStats = RawHavokInterop.ListFbxBones(tmpFileName).Result;
        Assert.AreEqual(105, resultStats.Count);

        File.Delete(tmpFileName);
    }

    [TestMethod]
    public void PapAndSklbToFbx()
    {
        var pap = Pap.FromFile("./TestFiles/animation.pap");
        Assert.AreNotEqual(0, pap.HavokData.Length);

        var sklb = Sklb.FromFile("./TestFiles/skeleton.sklb");
        Assert.AreNotEqual(0, sklb.HavokData.Length);

        string tmpFileName = Path.GetTempFileName();
        RawHavokInterop.CreateContainer(tmpFileName).Wait();

        string sklbHavok = Path.GetTempFileName();
        File.WriteAllBytes(sklbHavok, sklb.HavokData);
        RawHavokInterop.AddSkeleton(tmpFileName, sklbHavok, 0, tmpFileName).Wait();

        string papHavok = Path.GetTempFileName();
        File.WriteAllBytes(papHavok, pap.HavokData);
        RawHavokInterop.AddAnimation(tmpFileName, papHavok, 0, tmpFileName).Wait();

        var fbxPortResults = RawHavokInterop.ToFbxAnimation(tmpFileName, 0, 0, tmpFileName).Result;
        Assert.AreEqual(149, fbxPortResults.framesConverted);
        Assert.AreEqual(105, fbxPortResults.bonesConverted);

        var resultStats = RawHavokInterop.ListFbxBones(tmpFileName).Result;
        Assert.AreEqual(105, resultStats.Count);

        File.Delete(tmpFileName);
    }
}
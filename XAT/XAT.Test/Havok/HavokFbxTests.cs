using XAT.Game.Havok;

namespace XAT.Test.Havok;

[TestClass]
public class HavokFbxTests
{
    [TestMethod]
    public void ToFbxAnimation_Basic()
    {
        string tmpOutput = Path.GetTempFileName();

        var result = RawHavokInterop.ToFbxAnimation("./TestFiles/combined.hkt", 0, 0, tmpOutput).Result;
        Assert.AreEqual(105, result.bonesConverted);
        Assert.AreEqual(149, result.framesConverted);

        long newSize = new FileInfo(tmpOutput).Length;
        Assert.AreNotEqual(0, newSize);

        File.Delete(tmpOutput);
    }

    [TestMethod]
    public void ToFbxAnimation_Complex()
    {
        string tmpOutput = Path.GetTempFileName();
        
        var result = RawHavokInterop.ToFbxAnimation("./TestFiles/combined10.hkt", 0, 5, tmpOutput).Result;
        Assert.AreEqual(105, result.bonesConverted);
        Assert.AreEqual(149, result.framesConverted);

        long newSize = new FileInfo(tmpOutput).Length;
        Assert.AreNotEqual(0, newSize);

        File.Delete(tmpOutput);
    }

    [TestMethod]
    public void ToFbxSkeleton()
    {
        string tmpOutput = Path.GetTempFileName();

        var result = RawHavokInterop.ToFbxSkeleton("./TestFiles/skeleton.hkt", 0, tmpOutput).Result;
        Assert.AreEqual(105, result);

        long newSize = new FileInfo(tmpOutput).Length;
        Assert.AreNotEqual(0, newSize);

        var listResult = RawHavokInterop.ListFbxBones(tmpOutput).Result;
        Assert.AreEqual(105, listResult.Count);

        File.Delete(tmpOutput);
    }

    [TestMethod]
    public void ListFbxStacks_Simple()
    {
        var listResult = RawHavokInterop.ListFbxStacks("./TestFiles/basicanimation.fbx").Result;
        Assert.AreEqual(1, listResult.Count);
        Assert.AreEqual("n_root|n_root|xat|0", listResult[0]);
    }

    [TestMethod]
    public void ListFbxStacks_Blank()
    {
        var listResult = RawHavokInterop.ListFbxStacks("./TestFiles/blank.fbx").Result;
        Assert.AreEqual(0, listResult.Count);
    }

    [TestMethod]
    public void FromFbxSkeleton()
    {
        string tmpOutput = Path.GetTempFileName();

        RawHavokInterop.CreateContainer(tmpOutput).Wait();

        var toSkeletonResult = RawHavokInterop.FromFbxSkeleton(tmpOutput, "./TestFiles/basicanimation.fbx", null, tmpOutput).Result;
        Assert.AreEqual(105, toSkeletonResult);

        var finalStats = RawHavokInterop.GetStats(tmpOutput).Result;
        Assert.AreEqual(1, finalStats.skeletonCount);

        var boneList = RawHavokInterop.ListBones(tmpOutput, 0).Result;
        Assert.AreEqual(105, boneList.Count);

        File.Delete(tmpOutput);
    }

    [TestMethod]
    public void FromFbxSkeleton_Ordered()
    {
        string tmpOutput = Path.GetTempFileName();

        RawHavokInterop.CreateContainer(tmpOutput).Wait();

        var toSkeletonResult = RawHavokInterop.FromFbxSkeleton(tmpOutput, "./TestFiles/basicanimation.fbx", new List<string> { "n_root", "n_hara", "j_sebo_a" }, tmpOutput).Result;
        Assert.AreEqual(105, toSkeletonResult);

        var finalStats = RawHavokInterop.GetStats(tmpOutput).Result;
        Assert.AreEqual(1, finalStats.skeletonCount);

        var boneList = RawHavokInterop.ListBones(tmpOutput, 0).Result;
        Assert.AreEqual("n_root", boneList[0]);
        Assert.AreEqual("n_hara", boneList[1]);
        Assert.AreEqual("j_sebo_a", boneList[2]);
        Assert.AreEqual(105, boneList.Count);

        File.Delete(tmpOutput);
    }

    [TestMethod]
    public void FromFbxAnimation()
    {
        string tmpOutput = Path.GetTempFileName();

        RawHavokInterop.CreateContainer(tmpOutput).Wait();

        var toAnimResult = RawHavokInterop.FromFbxAnimation(tmpOutput, "./TestFiles/basicanimation.fbx", 0, "./TestFiles/skeleton.hkt", 0, null, tmpOutput).Result;
        Assert.AreEqual(149, toAnimResult.framesConverted);
        Assert.AreEqual(104, toAnimResult.bonesBound); // 104 because we don't bind n_root

        var finalStats = RawHavokInterop.GetStats(tmpOutput).Result;
        Assert.AreEqual(1, finalStats.animCount);
        Assert.AreEqual(1, finalStats.bindingCount);

        File.Delete(tmpOutput);
    }

    [TestMethod]
    public void FromFbxAnimation_ExcludeSomes()
    {
        string tmpOutput = Path.GetTempFileName();

        RawHavokInterop.CreateContainer(tmpOutput).Wait();

        var toAnimResult = RawHavokInterop.FromFbxAnimation(tmpOutput, "./TestFiles/basicanimation.fbx", 0, "./TestFiles/skeleton.hkt", 0, new List<string> { "n_hara" }, tmpOutput).Result;
        Assert.AreEqual(149, toAnimResult.framesConverted);
        Assert.AreEqual(103, toAnimResult.bonesBound); // 103 because we don't bind n_root or n_hara

        var finalStats = RawHavokInterop.GetStats(tmpOutput).Result;
        Assert.AreEqual(1, finalStats.animCount);
        Assert.AreEqual(1, finalStats.bindingCount);

        File.Delete(tmpOutput);
    }

    [TestMethod]
    public void ListFbxBones_Simple()
    {
        var listResult = RawHavokInterop.ListFbxBones("./TestFiles/basicanimation.fbx").Result;
        Assert.AreEqual(105, listResult.Count);
        Assert.AreEqual("n_root", listResult[0]);
        Assert.AreEqual("n_hara", listResult[1]);
    }

    [TestMethod]
    public void ListFbxBones_Blank()
    {
        var listResult = RawHavokInterop.ListFbxBones("./TestFiles/blank.fbx").Result;
        Assert.AreEqual(0, listResult.Count);
    }
}
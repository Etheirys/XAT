using XAT.Game.Havok;

namespace XAT.Test.Havok;

[TestClass]
public class HavokBasicTests
{
    [TestMethod]
    public void GetStats_Blank()
    {
        var result = RawHavokInterop.GetStats("./TestFiles/blank.hkt").Result;
        Assert.AreEqual(0, result.skeletonCount);
        Assert.AreEqual(0, result.animCount);
        Assert.AreEqual(0, result.bindingCount);
    }

    [TestMethod]
    public void GetStats_SkeletonOnly()
    {
        var result = RawHavokInterop.GetStats("./TestFiles/skeleton.hkt").Result;
        Assert.AreEqual(1, result.skeletonCount);
        Assert.AreEqual(0, result.animCount);
        Assert.AreEqual(0, result.bindingCount);
    }

    [TestMethod]
    public void GetStats_AnimationAndBinding()
    {
        var result = RawHavokInterop.GetStats("./TestFiles/animation.hkt").Result;
        Assert.AreEqual(0, result.skeletonCount);
        Assert.AreEqual(1, result.animCount);
        Assert.AreEqual(1, result.bindingCount);
    }

    [TestMethod]
    public void GetStats_Combined()
    {
        var result = RawHavokInterop.GetStats("./TestFiles/combined.hkt").Result;
        Assert.AreEqual(1, result.skeletonCount);
        Assert.AreEqual(1, result.animCount);
        Assert.AreEqual(1, result.bindingCount);
    }

    [TestMethod]
    public void GetStats_Combined10()
    {
        var result = RawHavokInterop.GetStats("./TestFiles/combined10.hkt").Result;
        Assert.AreEqual(1, result.skeletonCount);
        Assert.AreEqual(10, result.animCount);
        Assert.AreEqual(10, result.bindingCount);
    }

    [TestMethod]
    public void GetStats_XmlPackFile()
    {
        var result = RawHavokInterop.GetStats("./TestFiles/xmlpackfile.xml").Result;
        Assert.AreEqual(1, result.skeletonCount);
        Assert.AreEqual(1, result.animCount);
        Assert.AreEqual(1, result.bindingCount);
    }


    [TestMethod]
    public void CreateContainer()
    {
        string tmpOutput = Path.GetTempFileName();

        RawHavokInterop.CreateContainer(tmpOutput).Wait();

        long newSize = new FileInfo(tmpOutput).Length;
        Assert.AreNotEqual(0, newSize);

        var result = RawHavokInterop.GetStats(tmpOutput).Result;
        Assert.AreEqual(0, result.skeletonCount);
        Assert.AreEqual(0, result.animCount);
        Assert.AreEqual(0, result.bindingCount);

        File.Delete(tmpOutput);
    }

    [TestMethod]
    public void AddAnimation_Single()
    {         
        string tmpOutput = Path.GetTempFileName();

        var addResult = RawHavokInterop.AddAnimation("./TestFiles/skeleton.hkt", "./TestFiles/animation.hkt", 0, tmpOutput).Result;
        Assert.AreEqual(0, addResult.animIdx);
        Assert.AreEqual(0, addResult.bindingIdx);

        var result = RawHavokInterop.GetStats(tmpOutput).Result;
        Assert.AreEqual(1, result.skeletonCount);
        Assert.AreEqual(1, result.animCount);
        Assert.AreEqual(1, result.bindingCount);

        File.Delete(tmpOutput);
    }

    [TestMethod]
    public void AddAnimation_Double()
    {
        string tmpOutput = Path.GetTempFileName();
        File.WriteAllBytes(tmpOutput, File.ReadAllBytes("./TestFiles/skeleton.hkt"));

        var addResult = RawHavokInterop.AddAnimation(tmpOutput, "./TestFiles/animation.hkt", 0, tmpOutput).Result;
        Assert.AreEqual(0, addResult.animIdx);
        Assert.AreEqual(0, addResult.bindingIdx);

        var result = RawHavokInterop.GetStats(tmpOutput).Result;
        Assert.AreEqual(1, result.skeletonCount);
        Assert.AreEqual(1, result.animCount);
        Assert.AreEqual(1, result.bindingCount);

        addResult = RawHavokInterop.AddAnimation(tmpOutput, "./TestFiles/animation.hkt", 0, tmpOutput).Result;
        Assert.AreEqual(1, addResult.animIdx);
        Assert.AreEqual(1, addResult.bindingIdx);

        result = RawHavokInterop.GetStats(tmpOutput).Result;
        Assert.AreEqual(1, result.skeletonCount);
        Assert.AreEqual(2, result.animCount);
        Assert.AreEqual(2, result.bindingCount);

        File.Delete(tmpOutput);
    }

    [TestMethod]
    public void AddAnimation_Ten()
    {
        string tmpOutput = Path.GetTempFileName();
        File.WriteAllBytes(tmpOutput, File.ReadAllBytes("./TestFiles/skeleton.hkt"));

        for(int i = 0; i < 10; i++)
        {
            string sourceAnim = (i % 2 == 0) ? "./TestFiles/animation.hkt" : "./TestFiles/animation2.hkt";
            var addResult = RawHavokInterop.AddAnimation(tmpOutput, sourceAnim, 0, tmpOutput).Result;
            Assert.AreEqual(i, addResult.animIdx);
            Assert.AreEqual(i, addResult.bindingIdx);

            var result = RawHavokInterop.GetStats(tmpOutput).Result;
            Assert.AreEqual(1, result.skeletonCount);
            Assert.AreEqual(i+1, result.animCount);
            Assert.AreEqual(i+1, result.bindingCount);
        }

        File.Delete(tmpOutput);
    }

    [TestMethod]
    public void RemoveAnimation_Single()
    {
        string tmpOutput = Path.GetTempFileName();

        var removeResult = RawHavokInterop.RemoveAnimation("./TestFiles/animation.hkt", 0, tmpOutput).Result;
        Assert.AreEqual(0, removeResult.newAnimCount);
        Assert.AreEqual(0, removeResult.newBindingCount);

        var finalStats = RawHavokInterop.GetStats(tmpOutput).Result;
        Assert.AreEqual(0, finalStats.skeletonCount);
        Assert.AreEqual(0, finalStats.animCount);
        Assert.AreEqual(0, finalStats.bindingCount);

        File.Delete(tmpOutput);
    }

    [TestMethod]
    public void RemoveAnimation_Multi()
    {

        string tmpOutput = Path.GetTempFileName();
        File.WriteAllBytes(tmpOutput, File.ReadAllBytes("./TestFiles/combined10.hkt"));

        // Remove the one at the end
        var result = RawHavokInterop.RemoveAnimation(tmpOutput, 9, tmpOutput).Result;
        Assert.AreEqual(9, result.newAnimCount);
        Assert.AreEqual(9, result.newBindingCount);

        // Remove one in the middle
        result = RawHavokInterop.RemoveAnimation(tmpOutput, 5, tmpOutput).Result;
        Assert.AreEqual(8, result.newAnimCount);
        Assert.AreEqual(8, result.newBindingCount);

        // Remove first one
        result = RawHavokInterop.RemoveAnimation(tmpOutput, 0, tmpOutput).Result;
        Assert.AreEqual(7, result.newAnimCount);
        Assert.AreEqual(7, result.newBindingCount);

        var finalStats = RawHavokInterop.GetStats(tmpOutput).Result;
        Assert.AreEqual(1, finalStats.skeletonCount);
        Assert.AreEqual(7, finalStats.animCount);
        Assert.AreEqual(7, finalStats.bindingCount);

        File.Delete(tmpOutput);
    }

    [TestMethod]
    public void ReplaceAnimation_Single()
    {
        string tmpOutput = Path.GetTempFileName();

        var removeResult = RawHavokInterop.ReplaceAnimation("./TestFiles/combined10.hkt", 4, "./TestFiles/animation.hkt", 0, tmpOutput).Result;
        Assert.AreEqual(4, removeResult.animIdx);
        Assert.AreEqual(4, removeResult.bindingIdx);

        var finalStats = RawHavokInterop.GetStats(tmpOutput).Result;
        Assert.AreEqual(1, finalStats.skeletonCount);
        Assert.AreEqual(10, finalStats.animCount);
        Assert.AreEqual(10, finalStats.bindingCount);

        File.Delete(tmpOutput);
    }

    [TestMethod]
    public void RemoveSkeleton()
    {
        string tmpOutput = Path.GetTempFileName();

        var addResult = RawHavokInterop.RemoveSkeleton("./TestFiles/skeleton.hkt", 0, tmpOutput).Result;
        Assert.AreEqual(0, addResult);

        File.Delete(tmpOutput);
    }

    [TestMethod]
    public void AddSkeleton()
    {
        string tmpOutput = Path.GetTempFileName();

        var addResult = RawHavokInterop.AddSkeleton("./TestFiles/blank.hkt", "./TestFiles/skeleton.hkt", 0, tmpOutput).Result;
        Assert.AreEqual(1, addResult);

        var finalStats = RawHavokInterop.GetStats(tmpOutput).Result;
        Assert.AreEqual(1, finalStats.skeletonCount);

        File.Delete(tmpOutput);
    }

    [TestMethod]
    public void ListBones()
    {
        var listResult = RawHavokInterop.ListBones("./TestFiles/skeleton.hkt", 0).Result;
        Assert.AreEqual(105, listResult.Count);
        Assert.AreEqual("n_root", listResult[0]);
        Assert.AreEqual("n_hara", listResult[1]);
        Assert.AreEqual("n_sippo_e", listResult[104]);
    }

    [TestMethod]
    public void ToPackFile()
    {
        string tmpOutput = Path.GetTempFileName();

        RawHavokInterop.ToPackFile("./TestFiles/combined.hkt", tmpOutput).Wait();

        long newSize = new FileInfo(tmpOutput).Length;
        Assert.AreNotEqual(0, newSize);

        File.Delete(tmpOutput);
    }

    [TestMethod]
    public void ToTagFile()
    {
        string tmpOutput = Path.GetTempFileName();

        RawHavokInterop.ToTagFile("./TestFiles/combined.hkt", tmpOutput).Wait();

        long newSize = new FileInfo(tmpOutput).Length;
        Assert.AreNotEqual(0, newSize);

        var finalStats = RawHavokInterop.GetStats(tmpOutput).Result;
        Assert.AreEqual(1, finalStats.skeletonCount);
        Assert.AreEqual(1, finalStats.animCount);
        Assert.AreEqual(1, finalStats.bindingCount);

        File.Delete(tmpOutput);
    }

    [TestMethod]
    public void ToXMLFile()
    {
        string tmpOutput = Path.GetTempFileName();

        RawHavokInterop.ToXMLFile("./TestFiles/combined.hkt", tmpOutput).Wait();

        long newSize = new FileInfo(tmpOutput).Length;
        Assert.AreNotEqual(0, newSize);

        var finalStats = RawHavokInterop.GetStats(tmpOutput).Result;
        Assert.AreEqual(1, finalStats.skeletonCount);
        Assert.AreEqual(1, finalStats.animCount);
        Assert.AreEqual(1, finalStats.bindingCount);

        File.Delete(tmpOutput);
    }

}
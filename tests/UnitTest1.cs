namespace TestProject1;

[TestClass]
public class UnitTest1
{
    [TestMethod]
    public void TestMethod1()
    {
        Assert.IsTrue(Test.add(1, 1) == 2);
    }
    [TestMethod]
    public void TestMethod2()
    {
        Assert.IsTrue(Test.add(3, 1) == 4);
    }
}
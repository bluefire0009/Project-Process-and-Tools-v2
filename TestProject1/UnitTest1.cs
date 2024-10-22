using System.Diagnostics.CodeAnalysis;

namespace TestProject1;

[ExcludeFromCodeCoverage]
[TestClass]
public class UnitTest1
{
    [TestMethod]
    public void TestMethod1()
    {
        Test calc = new();
        Assert.IsTrue(calc.add(1, 1) == 2);
    }
    [TestMethod]
    public void TestMethod2()
    {
        Test calc = new();
        Assert.IsTrue(calc.add(3, 1) == 4);
    }
}
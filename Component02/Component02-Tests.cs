namespace Component01;
using NUnit.Framework;
using NUnit.Framework.Legacy;


[TestFixture]
public class Component02Tests
{
    // Test method marked for the sanity test suite
    // [Test, Category("Sanity")]
    [TestCase(1, 1, 2), Category("Sanity")]
    [TestCase(1, 2, 2), Category("Regression")]
    public void TestMethod1(int a, int b, int expectedResult)
    {
        ClassicAssert.AreEqual(expectedResult, a+b);  // Example assertion
        // Add more logic for your test
    }


    // Another test method marked for the sanity test suite
    [Test, Category("Sanity")]
    public void TestMethod2()
    {
        ClassicAssert.AreEqual(2, 2);  // Example assertion
                                       // Add more logic for your test
    }

    // Test method marked for the regression test suite
    [Test, Category("Regression")]
    public void TestMethod3()
    {
        Assert.That(3, Is.EqualTo(3));  // Example assertion
                                        // Add more logic for your test
    }

    // Another test method marked for both regression and sanity test suites
    [Test, Category("Regression"), Category("Sanity")]
    public void TestMethod4()
    {
        Assert.That(new object(), !Is.Null);  // Example assertion
                                              // This test applies to both suites
    }
}


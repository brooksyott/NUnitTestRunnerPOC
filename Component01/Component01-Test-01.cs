namespace Component01;

using System.Reflection.Metadata.Ecma335;
using NUnit.Framework;
using NUnit.Framework.Legacy;

using TestRunner.Framework;



[TestFixture(Description = "Component01 Test Class 01")]
[Property(TestFixture.Name, "Component-01-TestSuite-01")]
public class Component01Tests01
{
    // Test method marked for the sanity test suite
    // [Test, Category("Sanity")]
    [Test(Description = "Test ID: Dave-001 - Adding two positive numbers"), Category(TestCategories.Regression)]
    [Property(TestCaseProperties.TestID, "Dave-001")]
    public void TestMethod1()
    {
        ClassicAssert.IsTrue(1 == 1);  // Example assertion
        Console.WriteLine("TestMethod1");
        // Add more logic for your test
    }

    // Another test method marked for the sanity test suite
    [Test(Description = "Test ID is not known"), Category(TestCategories.Sanity), Category(TestCategories.Regression)]
    public void TestMethod2()
    {
        ClassicAssert.AreEqual(2, 2);  // Example assertion
                                       // Add more logic for your test
    }

    // Test method marked for the regression test suite
    // [Test, Category("Regression")]
    [Test]
    public void TestMethod3()
    {
        Assert.That(3, Is.EqualTo(3));  // Example assertion
                                        // Add more logic for your test
    }

    // Another test method marked for both regression and sanity test suites
    [Test, Category(TestCategories.Regression), Category(TestCategories.Sanity)]
    public void TestMethod4()
    {
        Assert.That(new object(), !Is.Null);  // Example assertion
                                              // This test applies to both suites
    }
}


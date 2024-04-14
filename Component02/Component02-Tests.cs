namespace Component02;
using NUnit.Framework;
using NUnit.Framework.Legacy;

public class ClassToTest {
    public async Task<int> AddAsync(int a, int b) {
        await Task.Delay(1000);
        return a + b;
    }
}

[TestFixture]
[Property("ComponentTest", "Component-02-TestSuite-01")]
public class Component02Tests
{
    // Test method marked for the sanity test suite
    // [Test, Category("Sanity")]
    // Must use Task, can't be async void
    [TestCase(1, 1, 2), Category("Sanity")]
    [TestCase(1, 2, 2), Category("Regression")]
    public async Task TestMethod1(int a, int b, int expectedResult)
    {
        var classToTest = new ClassToTest();
        var result = await classToTest.AddAsync(a, b);
        Assert.That(expectedResult, Is.EqualTo(result));  // Example assertion
    }

    // Another test method marked for the sanity test suite
    [Test, Category("Sanity")]
    public async Task TestMethod2()
    {
        var result = await Task.FromResult(true);
        Assert.That(result, Is.True);  // Example assertion
    }

    // Test method marked for the regression test suite
    [Test, Category("Regression")]
    public void TestMethod3()
    {
        Assert.That(3, Is.EqualTo(3));  // Example assertion
    }

    // Another test method marked for both regression and sanity test suites
    [Test, Category("Regression")]
    public void TestMethod4()
    {
        Assert.That(new object(), !Is.Null);  // Example assertion
        Assert.That(new object(), !Is.Null);  // Example assertion
    }
}


namespace TestRunner.Framework;

static public class TestCategories
{
    public const string Regression = "Regression";
    public const string Sanity = "Sanity";
    public const string NotSpecified = "All";

    public static List<String> All = [Regression, Sanity];

}

static public class TestCaseProperties {
    public const string TestID = "TestID";
    public const string Description = "Description";
    public const string Category = "Category";
    

}

static public class TestSuiteProperties {
    public const string ComponentTest = "ComponentTest";

}


static public class TestFixture {

    public const string Name = "ComponentTest";
}

static public class NUnitTestSuiteTypes {

    public const string TestFixture = "TestFixture";
}

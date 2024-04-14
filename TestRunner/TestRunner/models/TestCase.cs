namespace TestRunner;

public interface ITestCase
{
    // Should be a GUID, but can be any unique string
    public string ID { get; }
    public string Name { get; set; }

    public string CustomID { get; }

    // Testcase title and description
    public string Title { get; set; }
    public string Description { get; set; }

    public List<string> Categories { get; set; }

    // Excuting a test case should return a tuple of TestResultStatus and a list of logs
}

public class TestCase : ITestCase
{
    public string ID { get; set; }
    public string Name { get; set; }
    public string CustomID { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }

    public List<string> Categories { get; set; }
}
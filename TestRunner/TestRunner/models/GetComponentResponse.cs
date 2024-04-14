namespace TestRunner;
using System.Text.Json;
using System.Text.Json.Serialization;

public class GetTestLibrariesResponse
{
    public List<TestLibraryDetails> Components { get; set; } = new List<TestLibraryDetails>();
}

public class TestLibraryDetails
{
    public string Name { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int TestCount { get; set; }
    public List<ITestCase> Testcases { get; set; } = new List<ITestCase>();
}

// public class TestDetail
// {
//     public string Testset { get; set; }
//     public List<ITestCase> Testcases { get; set; }

// }

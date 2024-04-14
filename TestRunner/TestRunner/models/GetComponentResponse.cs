namespace TestRunner;
using System.Text.Json;
using System.Text.Json.Serialization;

public class GetComponentsResponse
{
    public List<ComponentDetails> Components { get; set; } = new List<ComponentDetails>();
}

public class ComponentDetails
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

namespace TestRunner;

using System.Xml;
using System.Reflection;
using System.Xml.Serialization;
using System.Text;
using NUnit;
using NUnit.Engine;

public class NUnitTestsRunner
{
    private readonly ITestEngine _testEngine;
    public TestFilter Filter;
    public ITestRunner Runner;

    public XmlDocument TestResultsXML { get; private set; }
    public XmlNode TestsXML { get; private set; }

    public TestRunRoot TestResults;

    public NUnitTestsRunner()
    {
        _testEngine = TestEngineActivator.CreateInstance();
    }

    public Boolean Load(IEnumerable<string> assemblyPaths, string category = "")
    {
        List<string> assemblyPathList = new List<string>();

        // Load the assemblies
        try
        {
            foreach (var assemblyPath in assemblyPaths)
            {
                PluginLoadContext loadContext = new PluginLoadContext(assemblyPath);
                Assembly assembly = loadContext.LoadFromAssemblyName(AssemblyName.GetAssemblyName(assemblyPath));
                var assemblyName = loadContext.LoadFromAssemblyName(AssemblyName.GetAssemblyName(assemblyPath));
                assemblyPathList.Add(assemblyName.Location);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return false;
        }

        ITestEngine engine = TestEngineActivator.CreateInstance();
        TestPackage package = new TestPackage(assemblyPathList);
        package.Settings.Add(EnginePackageSettings.InternalTraceLevel, "Off");

        Runner = engine.GetRunner(package);
        // Runner.Load();

        PrintTests();
        return true;
    }

    public TestRunRoot DeserializeRunResults()
    {
        try
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(TestResultsXML.OuterXml)))
            {
                XmlSerializer xs = new XmlSerializer(typeof(TestRunRoot), new XmlRootAttribute("test-run-root"));
                return (TestRunRoot)xs.Deserialize(stream);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return null;
        }
    }


    public async Task<Boolean> ExecuteAsync(string category = "", ITestEventListener eventListener = null)
    {
        // Initialize the master document root element
        TestResultsXML = new XmlDocument();
        XmlNode masterNode = TestResultsXML.CreateElement("test-run-root");
        TestResultsXML.AppendChild(masterNode);

        // Build the filter
        FilterBuilder filterBuilder = new FilterBuilder(category);
        Filter = filterBuilder.Build();

        var testrun = await Task<ITestRun>.Factory.StartNew(() => Runner.RunAsync(eventListener, Filter));
        var waitResult = testrun.Wait(10000000);
        var result = testrun.Result;
        XmlNode importedNode = TestResultsXML.ImportNode(result, true);
        masterNode.AppendChild(importedNode);

        PrintResults(category, true);

        return true;
    }

    public Boolean Execute(string category = "", ITestEventListener eventListener = null)
    {
        // Initialize the master document root element
        TestResultsXML = new XmlDocument();
        XmlNode masterNode = TestResultsXML.CreateElement("test-run-root");
        TestResultsXML.AppendChild(masterNode);

        // Build the filter
        FilterBuilder filterBuilder = new FilterBuilder(category);
        Filter = filterBuilder.Build();

        XmlNode result = Runner.Run(eventListener, Filter);
        XmlNode importedNode = TestResultsXML.ImportNode(result, true);
        masterNode.AppendChild(importedNode);

        PrintResults(category, true);

        return true;
    }


    public TestRun DeserializeRun(string category = "")
    {
        FilterBuilder filterBuilder = new FilterBuilder(category);
        Filter = filterBuilder.Build();

        TestsXML = Runner.Explore(Filter);
        // Console.WriteLine(TestsXML.OuterXml);

        try
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(TestsXML.OuterXml)))
            {
                XmlSerializer xs = new XmlSerializer(typeof(TestRun), new XmlRootAttribute("test-run"));
                return (TestRun)xs.Deserialize(stream);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return null;
        }
    }

    private void PrintTests()
    {
        PrintTests("");
        foreach (var category in TestTypes.All)
        {
            PrintTests(category);
        }
    }

    private void PrintTests(string category)
    {
        TestRun testRun = DeserializeRun(category);

        if (category == "")
        {
            category = "All";
        }
        
        Console.WriteLine("Category: {0}", category);

        foreach (TestSuite testSuite in testRun.TestSuites)
        {
            PrintTestSuites(testSuite);
        }
    }

    private void PrintTestSuites(TestSuite testSuite)
    {

        if (testSuite.Type == "TestFixture")
        {
            Console.WriteLine("\tTestSuite: {0}, Count: {1}", testSuite.Name, testSuite.TestCaseCount);
        }


        if (testSuite.TestCases.Count > 0)
        {
            foreach (var testcase in testSuite.TestCases)
            {
                var (testCaseId, description) = GetTestCaseInformation(testcase);
                Console.WriteLine("\t\tTestCase: {0}, Id: {1}, Custom Id: {2}, Description: {3}", testcase.MethodName, testcase.Id, testCaseId, description);
            }
        }

        if (testSuite.ChildSuites.Count > 0)
        {
            foreach (var suite in testSuite.ChildSuites)
            {
                PrintTestSuites(suite);
            }
        }
    }

    private void PrintResults(string category = "", Boolean detailed = false)
    {
        TestRunRoot tr = this.DeserializeRunResults();
        if (category == "")
        {
            category = "All";
        }

        Console.WriteLine("Category: {5}, Test Result: {0}, Passed: {1}, Failed: {2}, Skipped: {3}, Inconclusive: {4}", tr.TestRun.Result, tr.TestRun.Passed, tr.TestRun.Failed, tr.TestRun.Skipped, tr.TestRun.Inconclusive, category);

        if (detailed)
        {
            foreach (var testSuite in tr.TestRun.TestSuites)
            {
                PrintTestSuitesResults(testSuite);
            }
        }
    }

    private void PrintTestSuitesResults(TestSuite testSuite)
    {

        if (testSuite.Type == "TestFixture")
        {
            Console.WriteLine("\tTestSuite: {0}, Result: {1}, Passed: {2}, Failed: {3}, Skipped: {4}, Inconclusive: {5}", testSuite.Name, testSuite.Result, testSuite.Passed, testSuite.Failed, testSuite.Skipped, testSuite.Inconclusive);
        }


        if (testSuite.TestCases.Count > 0)
        {
            foreach (var testcase in testSuite.TestCases)
            {
                var (testCaseId, description) = GetTestCaseInformation(testcase);
                Console.WriteLine("\t\tTestCase: {0}, Id: {1}, Custom Id: {2}, Description: {3},  Result: {4}", testcase.MethodName, testcase.Id, testCaseId, description, testcase.Result);
            }
        }

        if (testSuite.ChildSuites.Count > 0)
        {
            foreach (var suite in testSuite.ChildSuites)
            {
                PrintTestSuitesResults(suite);
            }
        }
    }


    private (string, string) GetTestCaseInformation(TestCase testcase)
    {
        string description = "";
        string testCaseId = "";

        if (testcase?.Properties?.PropertyList == null)
        {
            return (testCaseId, description);
        }

        foreach (var property in testcase.Properties.PropertyList)
        {
            switch (property.Name)
            {
                case "Description":
                    description = property.Value;
                    break;
                case "TestID":
                    testCaseId = property.Value;
                    break;
            }
        }


        return (testCaseId, description);
    }


    public void SaveResults(XmlDocument document, string filePath)
    {
        document.Save(filePath);
    }
}

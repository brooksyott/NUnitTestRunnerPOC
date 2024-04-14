namespace TestRunner.NUnit;

using System.Xml;
using System.Reflection;
using System.Xml.Serialization;
using System.Text;
using global::NUnit.Engine;
using global::NUnit;

public class NUnitTestsRunner
{
    private readonly ITestEngine _testEngine;
    public TestFilter Filter;
    public ITestRunner Runner;

    public XmlDocument TestResultsXML { get; private set; }
    public XmlNode TestsXML { get; private set; }


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

    public NUnitTestRun DeserializeRunResults()
    {
        try
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(TestResultsXML.OuterXml)))
            {
                XmlSerializer xs = new XmlSerializer(typeof(NUnitTestRun), new XmlRootAttribute("test-run"));
                return (NUnitTestRun)xs.Deserialize(stream);
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
        // TestResultsXML = new XmlDocument();
        // XmlNode masterNode = TestResultsXML.CreateElement("test-run-root");
        // TestResultsXML.AppendChild(masterNode);

        // Build the filter
        NUnitFilterBuilder filterBuilder = new NUnitFilterBuilder(category);
        Filter = filterBuilder.Build();

        var testrun = await Task<ITestRun>.Factory.StartNew(() => Runner.RunAsync(eventListener, Filter));
        var waitResult = testrun.Wait(10000000);
        var result = testrun.Result;
        // XmlNode importedNode = TestResultsXML.ImportNode(result, true);
        // masterNode.AppendChild(importedNode);
        TestResultsXML = new XmlDocument();
        TestResultsXML.LoadXml(result.OuterXml);
        // TestResultsXML.Save("testResults.xml");

        PrintResults(category, true);

        return true;
    }

    public Boolean Execute(string category = "", ITestEventListener eventListener = null)
    {
        // Initialize the master document root element
        // TestResultsXML = new XmlDocument();
        // XmlNode masterNode = TestResultsXML.CreateElement("test-run-root");
        // TestResultsXML.AppendChild(masterNode);

        // Build the filter
        NUnitFilterBuilder filterBuilder = new NUnitFilterBuilder(category);
        Filter = filterBuilder.Build();

        XmlNode result = Runner.Run(eventListener, Filter);
        // XmlNode importedNode = TestResultsXML.ImportNode(result, true);
        // masterNode.AppendChild(importedNode);

        TestResultsXML = new XmlDocument();
        TestResultsXML.LoadXml(result.OuterXml);
        // TestResultsXML.Save("testResults.xml");


        PrintResults(category, true);

        return true;
    }


    public NUnitTestRun DeserializeRun(string category = "")
    {
        NUnitFilterBuilder filterBuilder = new NUnitFilterBuilder(category);
        Filter = filterBuilder.Build();

        TestsXML = Runner.Explore(Filter);
        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(TestsXML.OuterXml);
        xmlDoc.Save("deserializeRun.xml");

        // Console.WriteLine(TestsXML.OuterXml);

        try
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(TestsXML.OuterXml)))
            {
                XmlSerializer xs = new XmlSerializer(typeof(NUnitTestRun), new XmlRootAttribute("test-run"));
                return (NUnitTestRun)xs.Deserialize(stream);
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
        NUnitTestRun testRun = DeserializeRun(category);

        if (category == "")
        {
            category = "All";
        }

        Console.WriteLine("Category: {0}", category);

        foreach (NUnitTestSuite testSuite in testRun.TestSuites)
        {
            PrintTestSuites(testSuite);
        }
    }

    private void PrintTestSuites(NUnitTestSuite testSuite)
    {

        if (testSuite.Type == "TestFixture")
        {
            Console.WriteLine("\tTestSuite: {0}, Count: {1}", testSuite.Name, testSuite.TestCaseCount);
        }


        if (testSuite.TestCases.Count > 0)
        {
            foreach (var testcase in testSuite.TestCases)
            {
                var (testCaseId, description, categories) = GetTestCaseProperties(testcase);
                var categoriesListString = string.Join<string>(",", categories);
                Console.WriteLine("\t\tTestCase: {0}, Id: {1}, Custom Id: {2}, Test Categories: {3}, Description: {4}", testcase.MethodName, testcase.Id, testCaseId, categories, description);
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
        NUnitTestRun tr = this.DeserializeRunResults();
        if (category == "")
        {
            category = "All";
        }

        Console.WriteLine("Category: {5}, Test Result: {0}, Passed: {1}, Failed: {2}, Skipped: {3}, Inconclusive: {4}", tr.Result, tr.Passed, tr.Failed, tr.Skipped, tr.Inconclusive, category);

        if (detailed)
        {
            foreach (var testSuite in tr.TestSuites)
            {
                PrintTestSuitesResults(testSuite);
            }
        }
    }

    private void PrintTestSuitesResults(NUnitTestSuite testSuite)
    {

        if (testSuite.Type == "TestFixture")
        {
            Console.WriteLine("\tTestSuite: {0}, Result: {1}, Passed: {2}, Failed: {3}, Skipped: {4}, Inconclusive: {5}", testSuite.Name, testSuite.Result, testSuite.Passed, testSuite.Failed, testSuite.Skipped, testSuite.Inconclusive);
        }


        if (testSuite.TestCases.Count > 0)
        {
            foreach (var testcase in testSuite.TestCases)
            {
                var (testCaseId, description, categories) = GetTestCaseProperties(testcase);
                var categoriesListString = string.Join<string>(",", categories);
                Console.WriteLine("\t\tTestCase: {0}, Id: {1}, Custom Id: {2}, Description: {3}, Result: {4}", testcase.MethodName, testcase.Id, testCaseId, description, testcase.Result);
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


    public (string, string, List<String>) GetTestCaseProperties(NUnitTestCase testcase)
    {
        string description = "";
        string testCaseId = "";
        List<String> categories = new List<string>();

        if (testcase?.Properties?.PropertyList == null)
        {
            categories.Add("All");
            return (testCaseId, description, categories);
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
                case "Category":
                    categories.Add(property.Value);
                    break;
            }
        }


        return (testCaseId, description, categories);
    }

    public string GetTestSuiteProperties(NUnitTestSuite testSuite)
    {
        string componentTest = "";

        if (testSuite?.Properties?.PropertyList == null)
        {
            return componentTest;
        }

        foreach (var property in testSuite.Properties.PropertyList)
        {
            switch (property.Name)
            {
                case "ComponentTest":
                    componentTest = property.Value;
                    break;
            }
        }


        return componentTest;
    }



    public void SaveResults(XmlDocument document, string filePath)
    {
        document.Save(filePath);
    }
}

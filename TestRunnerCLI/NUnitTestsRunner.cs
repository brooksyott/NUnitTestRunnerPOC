using System;
using System.Collections.Generic;
using NUnit.Engine;
using NUnit.Engine.Extensibility;
using System.Xml;
using System.Reflection;
using NUnit.Framework;
using System.Runtime.Loader;
using System.Xml.Serialization;
using System.Text;
using System.Xml.Linq;
using System.Dynamic;
using Newtonsoft.Json;
using NUnit;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Api;
using System.Data.Common;

class PluginLoadContext : AssemblyLoadContext
{
    private AssemblyDependencyResolver _resolver;

    public PluginLoadContext(string pluginPath)
    {
        _resolver = new AssemblyDependencyResolver(pluginPath);
    }

    protected override Assembly Load(AssemblyName assemblyName)
    {
        string assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
        if (assemblyPath != null)
        {
            // return LoadFromAssemblyPath(assemblyPath);
            return Assembly.LoadFrom(assemblyPath);
        }

        return null;
    }

    protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
    {
        string libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
        if (libraryPath != null)
        {
            return LoadUnmanagedDllFromPath(libraryPath);
        }

        return IntPtr.Zero;
    }
}

public class TestAssemblyReference
{
    private static readonly Assembly Assembly = typeof(TestAssemblyReference).Assembly;
    private static readonly Type[] Types = Assembly.GetTypes().Where(t => t.Assembly.GetName().Name!.Equals(AssemblyName) && !t.IsDefined(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), inherit: false)).ToArray();

    private static readonly MethodInfo[] Methods = Types
      .SelectMany(t => t.GetMethods())
      .Where(m => m.GetCustomAttributes(typeof(TestAttribute), false)
        .Any()).ToArray();

    public static readonly string? AssemblyName = Assembly.GetName().Name;

    public static readonly IReadOnlyList<string> TestNames = Array
      .ConvertAll(Methods, m => $"{m.DeclaringType?.Namespace}.{m.DeclaringType?.Name}.{m.Name}")
      .ToList().AsReadOnly();
}

[Extension(EngineVersion = "3.4")]
public class MyEventListener : ITestEventListener
{
    /* ... */
    void ITestEventListener.OnTestEvent(string report)
    {
        // Console.WriteLine(report);
    }
}

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

    public TestRunRoot DeserializeRun()
    {
        TestsXML = Runner.Explore(TestFilter.Empty);

        try
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(TestsXML.OuterXml)))
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

    public async Task<Boolean> ExecuteAsync(string category = "")
    {
        // Initialize the master document root element
        TestResultsXML = new XmlDocument();
        XmlNode masterNode = TestResultsXML.CreateElement("test-run-root");
        TestResultsXML.AppendChild(masterNode);

        // Build the filter
        FilterBuilder filterBuilder = new FilterBuilder(category);
        Filter = filterBuilder.Build();

        var testrun = await Task<ITestRun>.Factory.StartNew(() => Runner.RunAsync(new MyEventListener(), Filter));
        var result = testrun.Result;
        XmlNode importedNode = TestResultsXML.ImportNode(result, true);
        masterNode.AppendChild(importedNode);

        return true;
    }

    public Boolean Execute(string category = "")
    {
        // Initialize the master document root element
        TestResultsXML = new XmlDocument();
        XmlNode masterNode = TestResultsXML.CreateElement("test-run-root");
        TestResultsXML.AppendChild(masterNode);

        // Build the filter
        FilterBuilder filterBuilder = new FilterBuilder(category);
        Filter = filterBuilder.Build();

        XmlNode result = Runner.Run(new MyEventListener(), Filter);
        XmlNode importedNode = TestResultsXML.ImportNode(result, true);
        masterNode.AppendChild(importedNode);

        PrintResults(category, true);

        return true;
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
                PrintTestSuites(testSuite);
            }
        }
    }

    private void PrintTestSuites(TestSuite testSuite)
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
                PrintTestSuites(suite);
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

    // public XmlDocument RunTest(string assemblyPath)
    // {
    //     XmlDocument masterDoc = new XmlDocument();
    //     XmlNode masterNode = masterDoc.CreateElement("test-run-root");

    //     // Initialize the master document root element
    //     masterDoc.AppendChild(masterNode);

    //     // Load the assembly
    //     PluginLoadContext loadContext = new PluginLoadContext(assemblyPath);
    //     Assembly assembly = loadContext.LoadFromAssemblyName(AssemblyName.GetAssemblyName(assemblyPath));
    //     var assemblyName = loadContext.LoadFromAssemblyName(AssemblyName.GetAssemblyName(assemblyPath));

    //     List<string> assemblyPathList = new List<string>();
    //     assemblyPathList.Add(assemblyPath);
    //     assemblyPathList.Add(assemblyPath);


    //     ITestEngine engine = TestEngineActivator.CreateInstance();
    //     // TestPackage package = new TestPackage(assemblyPath);
    //     TestPackage package = new TestPackage(assemblyPathList);
    //     package.Settings.Add(EnginePackageSettings.InternalTraceLevel, "Off");

    //     ITestRunner runner = engine.GetRunner(package);
    //     runner.Load();
    //     var testcaseCount = runner.CountTestCases(TestFilter.Empty);
    //     var testFilterService = engine.Services.GetService<ITestFilterService>();
    //     var testFilterBuilder = testFilterService.GetTestFilterBuilder();
    //     var builder = testFilterService.GetTestFilterBuilder();
    //     // testFilterBuilder.AddTest("bob");
    //     // foreach (var testName in TestAssemblyReference.TestNames)
    //     // {
    //     //     builder?.AddTest(testName);
    //     //     Console.WriteLine(testName);
    //     // }
    //     // builder.AddTest("Component01.Class1.MyTests.TestMethod1");
    //     TestFilter filters = testFilterBuilder.GetFilter();
    //     TestFilter sanityFilter = new TestFilter($"<filter><cat>Sanity</cat></filter>");

    //     var testcaseCount2 = runner.CountTestCases(sanityFilter);
    //     // foreach(var filter in filters.Text)
    //     // {
    //     //     Console.WriteLine(filter);
    //     // }

    //     var tree = runner.Explore(TestFilter.Empty);
    //     // XmlNode result = runner.Run(new NullListener(), filter);
    //     XmlNode result = runner.Run(new MyEventListener(), filters);

    //     // Console.WriteLine(result.OuterXml);

    //     XmlNode importedNode = masterDoc.ImportNode(result, true);
    //     XmlNode importedNode2 = masterDoc.ImportNode(result, true);
    //     masterNode.AppendChild(importedNode);
    //     masterNode.AppendChild(importedNode2);


    //     Console.WriteLine(masterNode.OuterXml);
    //     masterDoc.Save("testResults.xml");
    //     TestRunRoot tr;
    //     // XDocument doc = XDocument.Parse(masterNode.OuterXml);
    //     // string jsonText = JsonConvert.SerializeXNode(doc);
    //     // Console.WriteLine(jsonText);
    //     // dynamic dyn = JsonConvert.DeserializeObject<ExpandoObject>(jsonText);
    //     // var tr2 = JsonConvert.DeserializeObject<TestRunRoot>(jsonText);

    //     using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(masterDoc.OuterXml)))
    //     {
    //         Console.WriteLine("Deserializing");
    //         // XmlSerializer serializer = new XmlSerializer(typeof(TestRunRoot),"");
    //         // tr = (TestRunRoot)serializer.Deserialize(stream);


    //         XmlSerializer xs = new XmlSerializer(typeof(TestRunRoot), new XmlRootAttribute("test-run-root"));
    //         tr = (TestRunRoot)xs.Deserialize(stream);
    //         // dynamic dtr = xs.Deserialize(stream);
    //     }
    //     return masterDoc;
    // }


    // public XmlDocument RunTestsOrig(IEnumerable<string> assemblyPaths)
    // {
    //     XmlDocument masterDoc = new XmlDocument();
    //     XmlNode masterNode = masterDoc.CreateElement("TestRunRoot");

    //     // Initialize the master document root element
    //     masterDoc.AppendChild(masterNode);


    //     foreach (var assemblyPath in assemblyPaths)
    //     {
    //         if (! File.Exists(assemblyPath))
    //         {
    //             Console.WriteLine($"Assembly not found: {assemblyPath}");
    //         }

    //         var package = new TestPackage(assemblyPath);
    //         using (ITestEngine engine = TestEngineActivator.CreateInstance())
    //         using (ITestRunner runner = engine.GetRunner(package))
    //         {
    //             runner.Load();
    //             XmlNode result = runner.Run(null, TestFilter.Empty);

    //             // Import the result into the master document
    //             XmlNode importedNode = masterDoc.ImportNode(result, true);
    //             masterNode.AppendChild(importedNode);
    //         }
    //     }

    //     return masterDoc;
    // }

    public void SaveResults(XmlDocument document, string filePath)
    {
        document.Save(filePath);
    }
}

namespace TestRunner;

using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using global::NUnit.Framework.Internal;
using TestRunner.NUnit;


public partial class TestcaseRunnerService
{
    private readonly ILogger<TestcaseRunnerService> _logger;
    private readonly NUnitTestsRunner _runner;

    public TestcaseRunnerService(ILogger<TestcaseRunnerService> logger)
    {
        _logger = logger;
        _runner = new NUnitTestsRunner();
    }

    public void Load(IEnumerable<Component> components)
    {
        List<String> assemblyPaths = new List<String>();
        foreach (var component in components)
        {
            assemblyPaths.Add(component.Library);
        }
        _runner.Load(assemblyPaths);
    }

    public GetComponentsResponse GetComponentsDetails(bool withTestcases)
    {
        GetComponentsResponse response = new GetComponentsResponse();

        NUnitTestRun testRun = _runner.DeserializeRun();

        foreach (NUnitTestSuite testSuite in testRun.TestSuites)
        {
            ComponentDetails componentDetails = new ComponentDetails();
            MapTestSuites(testSuite, ref response, ref componentDetails);
            response.Components.Add(componentDetails);
        }

        return response;
    }

    private void MapTestSuites(NUnitTestSuite testSuite, ref GetComponentsResponse response, ref ComponentDetails componentDetails)
    {
        if (testSuite.Type == "TestFixture")
        {
            var componentTests = _runner.GetTestSuiteProperties(testSuite);
            componentDetails.Name = componentTests;
            componentDetails.TestCount = testSuite.TestCaseCount;
            _logger.LogDebug("Adding: TestFixture: {0}, Number of TestCases: {1}", componentTests, testSuite.TestCaseCount);
            // Console.WriteLine("\tAdding: TestFixture: {0}, Number of TestCases: {1}", componentTests, testSuite.TestCaseCount);
        }


        if (testSuite.TestCases.Count > 0)
        {
            foreach (var nUnitTestcase in testSuite.TestCases)
            {
                TestCase testCase = new TestCase();
                (testCase.CustomID, testCase.Description, testCase.Categories) = _runner.GetTestCaseProperties(nUnitTestcase);
                var categoriesListString = string.Join<string>(",", testCase.Categories);
                testCase.Name = nUnitTestcase.MethodName;
                testCase.ID = nUnitTestcase.Id;

                if (componentDetails != null)
                {
                    componentDetails.Testcases.Add(testCase);
                    _logger.LogDebug("Adding: TestCase: {0}, Id: {1}, Custom Id: {2}, Description: {3}, Categories: {4}", testCase.Name, testCase.ID, testCase.CustomID, testCase.Description, categoriesListString);
                    // Console.WriteLine("\t\tAdding: TestCase: {0}, Id: {1}, Custom Id: {2}, Description: {3}, Categories: {4}", testCase.Name, testCase.ID, testCase.CustomID, testCase.Description, categoriesListString);
                }
                else
                {
                    _logger.LogError("componentDetails is null");
                }
            }

        }


        if (testSuite.ChildSuites.Count > 0)
        {
            foreach (var suite in testSuite.ChildSuites)
            {
                MapTestSuites(suite, ref response, ref componentDetails);
            }
        }
    }
}
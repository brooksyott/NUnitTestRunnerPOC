using System;
using System.Xml;
using System.Xml.Linq;

public class Program
{
    static void Main(string[] args)
    {
        var runner = new NUnitTestsRunner();
        List<String> TestComponents = new List<String> {
            @"C:\Repos\Personal\XunitTestRunnerPOC\Component02\bin\Debug\net8.0\Component02.dll",
            @"C:\Repos\Personal\XunitTestRunnerPOC\Component01\bin\Debug\net8.0\Component01.dll"
        };

        runner.Load(TestComponents);
        Boolean success = runner.Execute();
        runner.TestResultsXML.Save("testResults.xml");

    }
}
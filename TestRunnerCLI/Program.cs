using System;
using System.Xml;
using System.Xml.Linq;
using System.CommandLine;

using TestRunner.NUnit;
using TestRunner;

public class Program
{
    static async Task Main(string[] args)
    {
        var testRunner = new TestRunnerCLI();
        var rootCommand = testRunner.CreateCommands();
        await rootCommand.InvokeAsync(args);

        return;

        var runner = new NUnitTestsRunner();
        List<String> TestComponents = new List<String> {
            @"../Component01/bin/Debug/net8.0/Component01.dll",
            @"../Component02/bin/Debug/net8.0/Component02.dll",
        };


        runner.Load(TestComponents);

        // Boolean success = await runner.ExecuteAsync(TestTypes.Sanity);
        // Boolean success = runner.Execute(TestTypes.Regresssion);
        // Boolean success = await runner.ExecuteAsync("", new TestRunEventListener());
        Boolean success = await runner.ExecuteAsync(TestTypes.Sanity);
        if (success)
        {
            runner.TestResultsXML.Save("testResults.xml");
        }
        else
        {
            Console.WriteLine("Error trying to run tests");
        }

        // await Task.CompletedTask;

    }
}
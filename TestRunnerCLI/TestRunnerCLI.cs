using System.CommandLine;
using TestRunner;
using Microsoft.Extensions.Configuration;

using Serilog;
using Microsoft.Extensions.Logging;
using Serilog.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Core;
using Serilog.Events;
using System.Text.Json;

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public partial class TestRunnerCLI
{
    private ServiceProvider _serviceProvider;
    private ILogger<TestRunnerCLI> _logger;

    private readonly LoggingLevelSwitch levelSwitch = new LoggingLevelSwitch();

    public void ChangeLogLevel(LogEventLevel newLevel)
    {
        levelSwitch.MinimumLevel = newLevel;
    }

    public bool ChangeLogLevel(string newLevel)
    {
        string level = newLevel.ToLower();
        switch (level)
        {
            case "verbose":
                ChangeLogLevel(LogEventLevel.Verbose);
                break;
            case "debug":
                ChangeLogLevel(LogEventLevel.Debug);
                break;
            case "information":
                ChangeLogLevel(LogEventLevel.Information);
                break;
            case "warning":
                ChangeLogLevel(LogEventLevel.Warning);
                break;
            case "error":
                ChangeLogLevel(LogEventLevel.Error);
                break;
            case "fatal":
                ChangeLogLevel(LogEventLevel.Fatal);
                break;
            default:
                ChangeLogLevel(LogEventLevel.Fatal);
                return false;
        }

        return true;
    }

    public RootCommand CreateCommands()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.ControlledBy(levelSwitch)
            .WriteTo.Console(outputTemplate:
                "{Timestamp:yy-MM-dd HH:mm:ss.fff}  {Level:u11} {SourceContext}: {Message}{NewLine}{Exception}")
            .CreateLogger();

        ChangeLogLevel(LogEventLevel.Fatal);


        // Set up dependency injection
        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);
        _serviceProvider = serviceCollection.BuildServiceProvider();
        _logger = _serviceProvider.GetService<ILogger<TestRunnerCLI>>();
        _logger.LogInformation("Starting Test Runner");

        var rootCommand = new RootCommand("Demo of flexible test runner");

        // // Add options to the root command that are common to all subcommands
        // // var verboseOption = new Option<bool>("--verbose", FalseDefault, "Show verbose output");
        // // verboseOption.AddAlias("-v");
        // // rootCommand.AddGlobalOption(verboseOption);

        var loglevelOption = new Option<string>("--log-level", DefaultLogLevel, "Configuration logging level");
        loglevelOption.AddAlias("-l");
        rootCommand.AddGlobalOption(loglevelOption);

        var configFileOption = new Option<string>("--config", "Configuration file for the test runner");
        configFileOption.AddAlias("-c");
        rootCommand.AddGlobalOption(configFileOption);

        // Add the list command
        AddGetCommand(ref rootCommand, configFileOption, loglevelOption);
        // AddRunCommand(ref rootCommand, configFileOption, loglevelOption);
        // AddTestSuiteCommand(ref rootCommand, configFileOption, loglevelOption);

        return rootCommand;
    }

    /// <summary>
    /// Binds all possible get commands to the root command
    /// </summary>
    /// <param name="rootCommand"></param>
    /// <param name="verboseOption"></param>
    /// <param name="configFileOption"></param>
    private void AddGetCommand(ref RootCommand rootCommand, Option<string> configFileOption, Option<string> logLevelOption)
    {
        var componentArgument = new Argument<string>(
           "components",
           "List available components");

        var showTestCasesOption = new Option<bool>("--show-testcases", FalseDefault, "Show test cases");
        showTestCasesOption.AddAlias("-s");

        var getCommand = new Command("get", "Get available components");
        getCommand.AddOption(showTestCasesOption);

        AddGetComponentsCommand(ref getCommand, configFileOption, logLevelOption);
        // AddGetTestsetsCommand(ref getCommand);
        // AddGetTestSuitesCommand(ref getCommand, configFileOption, logLevelOption);

        rootCommand.AddCommand(getCommand);
    }

    /// <summary>
    /// Add the get components command to the root command
    /// This will list all the components that are available, and the testcases that will be run if the verbose option is set
    /// Components are dynmically loaded from DLLs listed in the config file
    /// </summary>
    /// <param name="parentCommand"></param>
    /// <param name="verboseOption"></param>
    /// <param name="configFileOption"></param>
    private void AddGetComponentsCommand(ref Command parentCommand, Option<string> configFileOption, Option<string> logLevelOption)
    {
        var componentsCommand = new Command("components", "Get available components");
        var showTestCasesOption = new Option<bool>("--show-testcases", FalseDefault, "Show test cases");
        showTestCasesOption.AddAlias("-s");
        componentsCommand.AddOption(showTestCasesOption);

        componentsCommand.SetHandler((verbose, configFile, logLevel) =>
        {
            ChangeLogLevel(logLevel);

            var (testRunner, _) = NewTestcaseRunnerService(configFile);
            if (testRunner == null)
            {
                Console.WriteLine($"Could not create test runner: {configFile}");
                return;
            }

            var componentsResponse = testRunner.GetComponentsDetails(verbose);
            foreach (var component in componentsResponse.Components)
            {
                if (!verbose)
                {
                    Console.WriteLine($"Component: {component.Name}");
                    continue;
                }

                foreach (var testcase in component.Testcases)
                {
                    var categoriesListString = string.Join<string>(" & ", testcase.Categories);

                    Console.WriteLine($"Test Fixture: {component.Name}, Test Type: {categoriesListString}, ID: {testcase.ID}, Name: {testcase.Name} Title: {testcase.Description}");
                }
            }

        }, showTestCasesOption, configFileOption, logLevelOption);

        parentCommand.AddCommand(componentsCommand);
    }

    /// <summary>
    /// Build the test runner from the config file
    /// </summary>
    /// <param name="configFile"></param>
    /// <returns></returns>
    private (TestcaseRunnerService, Config) NewTestcaseRunnerService(string configFile, string component = null)
    {
        if (!File.Exists(configFile))
        {
            Console.WriteLine($"Config file not found: {configFile}");
            return (null, null);
        }
        var testConfig = GetConfig(configFile);
        var testRunner = _serviceProvider.GetService<TestcaseRunnerService>();
        if (testConfig?.TestLibraries == null)
        {
            _logger.LogError("No components found in the config file");
            return (null, null);
        }

        List<TestLibrary> componentsList;
        if (component == null)
        {
            componentsList = testConfig.TestLibraries;
        }
        else
        {
            componentsList = testConfig.TestLibraries.Where(c => c.Name == component).ToList();
        }

        testConfig.TestLibraries.Where(c => c.Name == component).ToList();
        testRunner.Load(componentsList);

        return (testRunner, testConfig);
    }

    /// <summary>
    /// Default value for the boolean options
    /// </summary>
    /// <returns></returns>
    private bool FalseDefault()
    {
        return false;
    }


    private string DefaultLogLevel()
    {
        return "error";
    }

    private void ConfigureServices(IServiceCollection services)
    {
        services.AddLogging(configure => configure.AddSerilog())
                .AddTransient<TestcaseRunnerService>();
    }

    /// <summary>
    /// Get the config from the config file
    /// </summary>
    /// <param name="configFile"></param>
    /// <returns></returns>
    private Config GetConfig(string configFile)
    {
        if (!File.Exists(configFile))
        {
            Console.WriteLine($"Config file not found: {configFile}");
            return null;
        }

        var fileExtension = Path.GetExtension(configFile);
        if (fileExtension == ".json")
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(configFile, optional: false);

            IConfiguration config = builder.Build();
            var testConfig = config.Get<Config>();
            return testConfig;
        }

        if (fileExtension == ".yml" || fileExtension == ".yaml")
        {
            // Add YAML deserialization here
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
            var yaml = File.ReadAllText(configFile);
            var testConfig = deserializer.Deserialize<Config>(yaml);
            return testConfig;
        }

        return null;
    }

}
namespace TestRunner;

using System;
using System.Collections.Generic;



/// <summary> 
/// Class for loading the configuration file
/// </summary>
public partial class Config
{
    public List<TestLibrary> TestLibraries { get; set; }
    public TestSuites TestSuites { get; set; }

    public LabConfig LabConfig { get; set; }
}


public partial class TestLibrary
{
    public string Name { get; set; }

    public string Library { get; set; }
}


public partial class TestSuites
{

    public List<TestsetConfig> PullRequest { get; set; }

    public List<TestsetConfig> Nightly { get; set; }

    public List<TestsetConfig> Weekend { get; set; }
}

public partial class TestsetConfig
{
    public string TestLibrary { get; set; }
    public List<TestsetComponentConfig> TestSets { get; set; }

}

public partial class TestsetComponentConfig
{
    public string TestLibrary { get; set; }

    public string TestSet { get; set; }
}

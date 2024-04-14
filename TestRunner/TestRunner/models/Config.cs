namespace TestRunner;

using System;
using System.Collections.Generic;



/// <summary> 
/// Class for loading the configuration file
/// </summary>
public partial class Config
{
    public List<Component> Components { get; set; }
    public TestSuites TestSuites { get; set; }

    public LabConfig LabConfig { get; set; }
}


public partial class Component
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
    public string Component { get; set; }
    public List<TestsetComponentConfig> TestSets { get; set; }

}

public partial class TestsetComponentConfig
{
    public string Component { get; set; }

    public string TestSet { get; set; }
}

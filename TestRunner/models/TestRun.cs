namespace TestRunner;
using System.Xml.Serialization;


[XmlRoot("test-run-root")]
public class TestRunRoot
{
    [XmlElement("test-run")]
    public TestRun TestRun { get; set; }
}

public class TestRun
{
    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlAttribute("runstate")]
    public string RunState { get; set; }

    [XmlAttribute("testcasecount")]
    public int TestCaseCount { get; set; }

    [XmlAttribute("result")]
    public string Result { get; set; }

    [XmlAttribute("total")]
    public int Total { get; set; }

    [XmlAttribute("passed")]
    public int Passed { get; set; }

    [XmlAttribute("failed")]
    public int Failed { get; set; }

    [XmlAttribute("warnings")]
    public int Warnings { get; set; }

    [XmlAttribute("inconclusive")]
    public int Inconclusive { get; set; }

    [XmlAttribute("skipped")]
    public int Skipped { get; set; }

    [XmlAttribute("asserts")]
    public int Asserts { get; set; }

    [XmlAttribute("engine-version")]
    public string EngineVersion { get; set; }

    [XmlAttribute("clr-version")]
    public string ClrVersion { get; set; }

    [XmlAttribute("start-time")]
    public String StartTime { get; set; }

    [XmlAttribute("end-time")]
    public String EndTime { get; set; }

    [XmlAttribute("duration")]
    public double Duration { get; set; }

    [XmlElement("command-line")]
    public string CommandLine { get; set; }

    [XmlElement("test-suite")]
    public List<TestSuite> TestSuites { get; set; }
}
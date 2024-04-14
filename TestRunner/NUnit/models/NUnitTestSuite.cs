namespace TestRunner.NUnit;
using System.Xml.Serialization;

public class NUnitTestSuite
{
    [XmlAttribute("type")]
    public string Type { get; set; }

    [XmlAttribute("id")]
    public string Id { get; set; }

    [XmlAttribute("name")]
    public string Name { get; set; }

    [XmlAttribute("fullname")]
    public string FullName { get; set; }

    [XmlAttribute("runstate")]
    public string RunState { get; set; }

    [XmlAttribute("testcasecount")]
    public int TestCaseCount { get; set; }

    [XmlAttribute("result")]
    public string Result { get; set; }

    [XmlAttribute("start-time")]
    public DateTime StartTime { get; set; }

    [XmlAttribute("end-time")]
    public DateTime EndTime { get; set; }

    [XmlAttribute("duration")]
    public double Duration { get; set; }

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

    [XmlElement("environment")]
    public NUnitEnvironment Environment { get; set; }

    [XmlElement("settings")]
    public NUnitSettings Settings { get; set; }

    [XmlElement("properties")]
    public NUnitProperties Properties { get; set; }

    [XmlElement("failure")]
    public NUnitFailure Failure { get; set; }

    [XmlElement("test-suite")]
    public List<NUnitTestSuite> ChildSuites { get; set; }

    [XmlElement("test-case")]
    public List<NUnitTestCase> TestCases { get; set; }
}
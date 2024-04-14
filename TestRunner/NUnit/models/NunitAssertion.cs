namespace TestRunner.NUnit;
using System.Xml.Serialization;

public class NUnitAssertions
{
    [XmlElement("assertion")]
    public List<NUnitAssertion> AssertionList { get; set; }
}

public class NUnitAssertion
{
    [XmlAttribute("result")]
    public string Result { get; set; }

    [XmlElement("message")]
    public string Message { get; set; }

    [XmlElement("stack-trace")]
    public string StackTrace { get; set; }
}
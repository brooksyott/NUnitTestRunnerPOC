namespace TestRunner;
using System.Xml.Serialization;

public class Assertions
{
    [XmlElement("assertion")]
    public List<Assertion> AssertionList { get; set; }
}

public class Assertion
{
    [XmlAttribute("result")]
    public string Result { get; set; }

    [XmlElement("message")]
    public string Message { get; set; }

    [XmlElement("stack-trace")]
    public string StackTrace { get; set; }
}
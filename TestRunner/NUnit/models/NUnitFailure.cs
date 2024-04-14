namespace TestRunner.NUnit;
using System.Xml.Serialization;

public class NUnitFailure
{
    [XmlElement("message")]
    public string Message { get; set; }

    [XmlElement("stack-trace")]
    public string StackTrace { get; set; }
}
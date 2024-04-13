namespace TestRunner;
using System.Xml.Serialization;

public class Failure
{
    [XmlElement("message")]
    public string Message { get; set; }

    [XmlElement("stack-trace")]
    public string StackTrace { get; set; }
}
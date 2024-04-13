using System.Xml.Serialization;

public class TestCase
{
    [XmlAttribute("id")]
    public string Id { get; set; }

    [XmlAttribute("name")]
    public string Name { get; set; }

    [XmlAttribute("fullname")]
    public string FullName { get; set; }

    [XmlAttribute("methodname")]
    public string MethodName { get; set; }

    [XmlAttribute("classname")]
    public string ClassName { get; set; }

    [XmlAttribute("runstate")]
    public string RunState { get; set; }

    [XmlAttribute("seed")]
    public long Seed { get; set; }

    [XmlAttribute("result")]
    public string Result { get; set; }

    [XmlAttribute("start-time")]
    public DateTime StartTime { get; set; }

    [XmlAttribute("end-time")]
    public DateTime EndTime { get; set; }

    [XmlAttribute("duration")]
    public double Duration { get; set; }

    [XmlAttribute("asserts")]
    public int Asserts { get; set; }

    [XmlElement("properties")]
    public Properties Properties { get; set; }

    [XmlElement("output")]
    public string Output { get; set; }

    [XmlElement("failure")]
    public Failure Failure { get; set; }

    [XmlElement("assertions")]
    public Assertions Assertions { get; set; }
}
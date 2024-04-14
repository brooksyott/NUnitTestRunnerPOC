namespace TestRunner.NUnit;
using System.Xml.Serialization;

public class NUnitProperties
{
    [XmlElement("property")]
    public List<NUnitProperty> PropertyList { get; set; }
}

public class NUnitProperty
{
    [XmlAttribute("name")]
    public string Name { get; set; }

    [XmlAttribute("value")]
    public string Value { get; set; }
}

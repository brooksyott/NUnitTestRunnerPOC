namespace TestRunner;
using System.Xml.Serialization;

public class Properties
{
    [XmlElement("property")]
    public List<Property> PropertyList { get; set; }
}

public class Property
{
    [XmlAttribute("name")]
    public string Name { get; set; }

    [XmlAttribute("value")]
    public string Value { get; set; }
}

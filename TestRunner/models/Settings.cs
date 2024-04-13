namespace TestRunner;
using System.Xml.Serialization;

public class Settings
{
    [XmlElement("setting")]
    public List<Setting> SettingList { get; set; }
}

public class Setting
{
    [XmlAttribute("name")]
    public string Name { get; set; }

    [XmlAttribute("value")]
    public string Value { get; set; }
}

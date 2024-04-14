namespace TestRunner.NUnit;
using System.Xml.Serialization;

public class NUnitSettings
{
    [XmlElement("setting")]
    public List<NUnitSetting> SettingList { get; set; }
}

public class NUnitSetting
{
    [XmlAttribute("name")]
    public string Name { get; set; }

    [XmlAttribute("value")]
    public string Value { get; set; }
}

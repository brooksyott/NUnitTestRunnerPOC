namespace TestRunner.NUnit;

using System.Xml.Serialization;
using global::NUnit.Engine;


[Serializable, XmlRoot("filter")]
public class NUnitFilterBuilder
{
    [XmlElement("cat")]
    public string Category { get; set; }

    public NUnitFilterBuilder()
    {
    }

    public NUnitFilterBuilder(string category)
    {
        Category = category;
    }

    public TestFilter Build()
    {
        if (string.IsNullOrEmpty(Category))
        {
            return TestFilter.Empty;
        }

        string xml = "";
        var serializer = new XmlSerializer(typeof(NUnitFilterBuilder));
        using (var writer = new StringWriter())
        {
            serializer.Serialize(writer, this);
            xml = writer.ToString();
        }

        return new TestFilter(xml);
    }
}
namespace TestRunner;

using System.Xml.Serialization;
using NUnit.Engine;


[Serializable, XmlRoot("filter")]
public class FilterBuilder
{
    [XmlElement("cat")]
    public string Category { get; set; }

    public FilterBuilder()
    {
    }

    public FilterBuilder(string category)
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
        var serializer = new XmlSerializer(typeof(FilterBuilder));
        using (var writer = new StringWriter())
        {
            serializer.Serialize(writer, this);
            xml = writer.ToString();
        }

        return new TestFilter(xml);
    }
}
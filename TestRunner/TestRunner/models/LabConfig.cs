public partial class LabConfig
{
    public string Name { get; set; }

    public Networking Networking { get; set; }
}

public partial class Networking
{
    public string Dns { get; set; }

    public string Ip { get; set; }
}
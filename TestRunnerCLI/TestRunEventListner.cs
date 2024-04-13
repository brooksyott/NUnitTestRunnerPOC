
using System.Runtime.CompilerServices;
using NUnit.Engine;
using NUnit.Engine.Extensibility;

[NUnit.Engine.Extensibility.Extension(EngineVersion = "3.4")]
public class TestRunEventListener : ITestEventListener
{
    /* ... */
    void ITestEventListener.OnTestEvent(string report)
    {
        Console.WriteLine("**************************************************");
        Console.WriteLine(report);
        Console.WriteLine("**************************************************");
    }
}

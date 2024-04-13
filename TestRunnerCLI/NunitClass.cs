// using System;
// using System.Xml.Serialization;
// using System.Collections.Generic;

// [Serializable, XmlRoot("test-run-root")]
// public class TestResultsRoot
// {
//     [XmlElement("test-run")]
//     public TestRun TestRun { get; set; }
// }

// public class TestRun
// {
//     [XmlAttribute("id")]
//     public int Id { get; set; }

//     [XmlAttribute("name")]
//     public string Name { get; set; }

//     [XmlAttribute("fullname")]
//     public string FullName { get; set; }

//     [XmlAttribute("runstate")]
//     public string RunState { get; set; }

//     [XmlAttribute("testcasecount")]
//     public int TestCaseCount { get; set; }

//     [XmlAttribute("result")]
//     public string Result { get; set; }

//     [XmlAttribute("total")]
//     public int Total { get; set; }

//     [XmlAttribute("passed")]
//     public int Passed { get; set; }

//     [XmlAttribute("failed")]
//     public int Failed { get; set; }

//     [XmlAttribute("warnings")]
//     public int Warnings { get; set; }

//     [XmlAttribute("inconclusive")]
//     public int Inconclusive { get; set; }

//     [XmlAttribute("skipped")]
//     public int Skipped { get; set; }

//     [XmlAttribute("asserts")]
//     public int Asserts { get; set; }

//     [XmlAttribute("engine-version")]
//     public string EngineVersion { get; set; }

//     [XmlAttribute("clr-version")]
//     public string ClrVersion { get; set; }

//     [XmlAttribute("start-time")]
//     public String StartTime { get; set; }

//     [XmlAttribute("end-time")]
//     public String EndTime { get; set; }

//     [XmlAttribute("duration")]
//     public double Duration { get; set; }

//     [XmlElement("command-line")]
//     public string CommandLine { get; set; }

//     [XmlElement("test-suite")]
//     public List<TestSuite> TestSuites { get; set; }
// }

// // Represents <test-suite> elements
// public class TestSuite
// {
//     [XmlAttribute("type")]
//     public string Type { get; set; }

//     [XmlAttribute("id")]
//     public string Id { get; set; }

//     [XmlAttribute("name")]
//     public string Name { get; set; }

//     [XmlAttribute("fullname")]
//     public string FullName { get; set; }

//     [XmlAttribute("runstate")]
//     public string RunState { get; set; }

//     [XmlAttribute("testcasecount")]
//     public int TestCaseCount { get; set; }

//     [XmlAttribute("result")]
//     public string Result { get; set; }

//     [XmlAttribute("start-time")]
//     public String StartTime { get; set; }

//     [XmlAttribute("end-time")]
//     public String EndTime { get; set; }

//     [XmlAttribute("duration")]
//     public double Duration { get; set; }

//     [XmlAttribute("total")]
//     public int Total { get; set; }

//     [XmlAttribute("passed")]
//     public int Passed { get; set; }

//     [XmlAttribute("failed")]
//     public int Failed { get; set; }

//     [XmlAttribute("warnings")]
//     public int Warnings { get; set; }

//     [XmlAttribute("inconclusive")]
//     public int Inconclusive { get; set; }

//     [XmlAttribute("skipped")]
//     public int Skipped { get; set; }

//     [XmlAttribute("asserts")]
//     public int Asserts { get; set; }

//     [XmlElement("test-case")]
//     public List<TestCase> TestCases { get; set; }

//     [XmlElement("test-suite")]
//     public List<TestSuite> SubSuites { get; set; }

//     [XmlElement("environment")]
//     public Environment Environment { get; set; }

//     [XmlElement("settings")]
//     public Settings Settings { get; set; }

//     [XmlElement("properties")]
//     public List<Property> Properties { get; set; }
// }

// // Represents <test-case> elements
// // public class TestCase
// // {
// //     [XmlAttribute("id")]
// //     public string Id { get; set; }

// //     [XmlAttribute("name")]
// //     public string Name { get; set; }

// //     [XmlAttribute("fullname")]
// //     public string FullName { get; set; }

// //     [XmlAttribute("methodname")]
// //     public string MethodName { get; set; }

// //     [XmlAttribute("classname")]
// //     public string ClassName { get; set; }

// //     [XmlAttribute("runstate")]
// //     public string RunState { get; set; }

// //     [XmlAttribute("seed")]
// //     public long Seed { get; set; }

// //     [XmlAttribute("result")]
// //     public string Result { get; set; }

// //     [XmlAttribute("start-time")]
// //     public String StartTime { get; set; }

// //     [XmlAttribute("end-time")]
// //     public String EndTime { get; set; }

// //     [XmlAttribute("duration")]
// //     public double Duration { get; set; }

// //     [XmlAttribute("asserts")]
// //     public int Asserts { get; set; }

// //     [XmlElement("output")]
// //     public string Output { get; set; }

// //     [XmlElement("properties")]
// //     public List<Property> Properties { get; set; }
// // }

// // Represents environment information within <test-suite>
// public class Environment
// {
//     [XmlAttribute("framework-version")]
//     public string FrameworkVersion { get; set; }

//     [XmlAttribute("clr-version")]
//     public string ClrVersion { get; set; }

//     [XmlAttribute("os-version")]
//     public string OSVersion { get; set; }

//     [XmlAttribute("platform")]
//     public string Platform { get; set; }

//     [XmlAttribute("cwd")]
//     public string CurrentWorkingDirectory { get; set; }

//     [XmlAttribute("machine-name")]
//     public string MachineName { get; set; }

//     [XmlAttribute("user")]
//     public string User { get; set; }

//     [XmlAttribute("user-domain")]
//     public string UserDomain { get; set; }

//     [XmlAttribute("culture")]
//     public string Culture { get; set; }

//     [XmlAttribute("uiculture")]
//     public string UICulture { get; set; }

//     [XmlAttribute("os-architecture")]
//     public string OSArchitecture { get; set; }
// }

// // Represents <settings> within <test-suite>
// public class Settings
// {
//     [XmlElement("setting")]
//     public List<Setting> Setting { get; set; }
// }

// public class Setting
// {
//     [XmlAttribute("name")]
//     public string Name { get; set; }

//     [XmlAttribute("value")]
//     public string Value { get; set; }
// }



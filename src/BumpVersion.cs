namespace Ivy.Versioning
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.XPath;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;
    using NuGet.Versioning;
    public class BumpVersion : Task
    {
        public override bool Execute()
        {
            /*
            Debugger.Launch();
            if (!Debugger.IsAttached)
            {
                Thread.Sleep(100);
                Log.LogWarning("Waiting debugger...");
            }
            */
            Log.LogMessage(MessageImportance.Low, "Ivy Version task started");
            try
            {
                var proj = XDocument.Load(ProjectPath, LoadOptions.PreserveWhitespace);

                if (TryBump(proj, "Version"))
                {
                    Log.LogMessage(MessageImportance.Low, "Saving project file");
                    var setting = new XmlWriterSettings
                    {
                        OmitXmlDeclaration = true,
                        Indent = true
                    };
                    using (var stream = File.Create(ProjectPath))
                    using (var xml = XmlWriter.Create(stream, setting))
                    {
                        xml.Flush();
                        proj.Save(xml);
                    }
                }
            }
            catch (Exception e)
            {
                Log.LogErrorFromException(e);
                return false;
            }

            return true;
        }
        private bool TryBump(XDocument proj, string tagName)
        {
            var defaultNamespace = proj.Root.GetDefaultNamespace();
            var defaultNamespacePrefix = "ns";
            var xmlNamespaceManager = new XmlNamespaceManager(new NameTable());
            Log.LogMessage(MessageImportance.High, $"ns: {defaultNamespace.NamespaceName}");

            xmlNamespaceManager.AddNamespace(defaultNamespacePrefix, defaultNamespace.NamespaceName);

            var element = proj.Root.XPathSelectElement(
                $"{defaultNamespacePrefix}:PropertyGroup/{defaultNamespacePrefix}:{tagName}", xmlNamespaceManager);

            if (element == null)
                return false;
            var oldVersion = new NuGetVersion(element.Value);
            Log.LogMessage(MessageImportance.Low, $"Old {tagName} is {element.Value}");

            int GetNextValue(int oldValue, bool bump, bool reset)
            {
                if (reset)
                    return 0;
                if (bump)
                    return oldValue + 1;
                return oldValue;
            }

            var major = GetNextValue(oldVersion.Major, BumpMajor, ResetMajor);
            var minor = GetNextValue(oldVersion.Minor, BumpMinor, ResetMinor);
            var patch = GetNextValue(oldVersion.Patch, BumpPatch, ResetPatch);
            var revision = GetNextValue(oldVersion.Revision, BumpRevision, ResetRevision);

            var labels = oldVersion.ReleaseLabels.ToList();
            if (!string.IsNullOrEmpty(ResetLabel))
            {
                var regex = new Regex($"^{Regex.Escape(ResetLabel)}(\\d*)$");
                var collection = 
                    from label in labels
                    let match = regex.Match(label)
                    where match.Success
                    select label;
                foreach (var label in collection)
                {
                    labels.Remove(label);
                    break;
                }
            }

            // Find and modify the release label selected with `BumpLabel`
            // If ResetLabel is true, remove only the specified label.
            if (!string.IsNullOrEmpty(BumpLabel) && BumpLabel != ResetLabel)
            {
                var regex = new Regex($"^{Regex.Escape(BumpLabel)}(\\d*)$");
                var value = 0;
                foreach (var label in labels)
                {
                    var match = regex.Match(label);
                    if (!match.Success) continue;
                    if (!string.IsNullOrEmpty(match.Groups[1].Value))
                        value = int.Parse(match.Groups[1].Value);
                    labels.Remove(label);
                    break;
                }

                value++;
                labels.Add(BumpLabel + value.ToString(new string('0', LabelDigits)));
            }

            var newVersion = new NuGetVersion(major, minor, patch, revision, labels, oldVersion.Metadata);

            // Modify the project file and set output properties
            if (newVersion == oldVersion) 
                return false;
            var newVersionStr = newVersion.ToString();
            Log.LogMessage(MessageImportance.High, $"Changing {tagName} to {newVersionStr}...");
            element.Value = newVersionStr;
            GetRequiredPropertyInfo("New" + tagName).SetValue(this, newVersionStr);
            return true;

        }

        private PropertyInfo GetRequiredPropertyInfo(string propertyName)
        {
            return GetType().GetProperty(propertyName) ??
                   throw new Exception($"Property {propertyName} is missing from type {GetType().Name}");
        }

        [Required] public string ProjectPath { get; set; }

        public string Configuration { get; set; }

        public bool BumpMajor { get; set; }

        public bool BumpMinor { get; set; }

        public bool BumpPatch { get; set; }

        public bool BumpRevision { get; set; }

        public string BumpLabel { get; set; }

        public bool ResetMajor { get; set; }

        public bool ResetMinor { get; set; }

        public bool ResetPatch { get; set; }

        public bool ResetRevision { get; set; }

        public string ResetLabel { get; set; }

        public int LabelDigits { get; set; } = 6;

        [Output] public string NewVersion { get; set; }
    }
}
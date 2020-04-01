namespace Ivy.Versioning
{
    using System.Collections.Generic;

    public class SettingsCollection : Settings
    {
        public Dictionary<string, Settings> Configurations { get; set; }
    }
}
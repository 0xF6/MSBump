namespace Ivy.Versioning
{
    using Microsoft.Build.Framework;
    public partial class BumpVersion
    {
        [Required] 
        public string ProjectPath { get; set; }
        [Output]
        public string NewVersion { get; set; }


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
    }
}
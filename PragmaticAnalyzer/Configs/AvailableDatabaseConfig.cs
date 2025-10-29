using PragmaticAnalyzer.DTO;
using System.Text.Json.Serialization;

namespace PragmaticAnalyzer.Configs
{
    public class AvailableDatabaseConfig(string displayedName, string fullName, long sizeBytes, DateTime lastModified, DataType detectedType)
    {
        [JsonIgnore]
        public bool IsChecked { get; set; } = true;
        public string DisplayedName { get; private set; } = displayedName;
        public string FullName { get; private set; } = fullName;
        public long SizeBytes { get; private set; } = sizeBytes;
        public DateTime LastModified { get; private set; } = lastModified;
        public DataType DetectedType { get; private set; } = detectedType;
    }
}
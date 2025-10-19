using PragmaticAnalyzer.DTO;

namespace PragmaticAnalyzer.Configs
{
    public class AvailableDatabaseConfig
    {
        public DataType Name { get; set; }
        public string FullName { get; set; }
        public long SizeBytes { get; set; }
        public bool IsChecked { get; set; }
        public DateTime LastModified { get; set; }
    }
}
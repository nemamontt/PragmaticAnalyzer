using PragmaticAnalyzer.Abstractions;
using PragmaticAnalyzer.Core;

namespace PragmaticAnalyzer.Databases
{
    public class Threat : ViewModelBase, IDatabase
    {
        public Guid GuidId { get; set; } = Guid.NewGuid();
        public string Id { get => Get<string>(); set => Set(value); }
        public string Name { get => Get<string>(); set => Set(value); }
        public string Description { get => Get<string>(); set => Set(value); }
        public string Source { get => Get<string>(); set => Set(value); }
        public string ObjectInfluence { get => Get<string>(); set => Set(value); }
        public string PrivacyViolation { get => Get<string>(); set => Set(value); }
        public string IntegrityViolation { get => Get<string>(); set => Set(value); }
        public string AccessibilityViolation { get => Get<string>(); set => Set(value); }
        public string DateInclusion { get => Get<string>(); set => Set(value); }
        public string DateChange { get => Get<string>(); set => Set(value); }
    }
}
using PragmaticAnalyzer.Abstractions;
using PragmaticAnalyzer.Core;
using System.Text.Json.Serialization;

namespace PragmaticAnalyzer.Databases
{
    public class ProtectionMeasure : ViewModelBase, IDatabase
    {
        public Guid GuidId { get; } = Guid.NewGuid();
        public string NameGroup { get => Get<string>(); set => Set(value); }
        public string Name { get => Get<string>(); set => Set(value); }
        public string Number { get => Get<string>(); set => Set(value); }
        public string Description { get => Get<string>(); set => Set(value); }
        public string SecurityClasses { get => Get<string>(); set => Set(value); }
        [JsonIgnore]
        public string DispayedName => Name + "." + Number;
    }
}
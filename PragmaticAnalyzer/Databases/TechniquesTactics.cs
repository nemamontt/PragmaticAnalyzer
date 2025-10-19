using PragmaticAnalyzer.Abstractions;
using PragmaticAnalyzer.Core;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace PragmaticAnalyzer.Databases
{
    public interface IEntityTIT
    {
        string Name { get; set; }
        string Description { get; set; }
    }

    public class Tactic : ViewModelBase, IEntityTIT, IDatabase
    {
        public Guid GuidId { get; } = Guid.NewGuid();
        public string Name { get => Get<string>(); set => Set(value); }
        public string Description { get => Get<string>(); set => Set(value); }
        public ObservableCollection<Technique> Techniques { get; set; }
        [JsonIgnore]
        public string DisplayName => $"Тактика: {Name}";
    }

    public class Technique : ViewModelBase, IEntityTIT, IDatabase
    {
        public Guid GuidId { get; } = Guid.NewGuid();
        public string Name { get => Get<string>(); set => Set(value); }
        public string Description { get => Get<string>(); set => Set(value); }
        [JsonIgnore]
        public string DisplayName => $"Техника: {Name}";
    }
}
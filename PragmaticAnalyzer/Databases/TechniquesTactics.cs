using PragmaticAnalyzer.Abstractions;
using PragmaticAnalyzer.Core;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace PragmaticAnalyzer.Databases
{
    public interface IEntityTIT
    {
        string Name { get; set; }
        string Description { get; set; }
    }

    public class Tactic : ViewModelBase, IEntityTIT, IHasId, IHasDescription
    {
        public Guid GuidId { get; set; } = Guid.NewGuid();

        public string Name { get => Get<string>(); set => Set(value); }

        [Description("Описание")]
        public string Description { get => Get<string>(); set => Set(value); }

        public ObservableCollection<Technique> Techniques { get; set; }

        [JsonIgnore]
        [Description("Наименование")]
        public string DisplayName => $"Тактика: {Name}";
    }

    public class Technique : ViewModelBase, IEntityTIT, IHasId
    {
        public Guid GuidId { get; set; } = Guid.NewGuid();

        public string Name { get => Get<string>(); set => Set(value); }

        [Description("Описание")]
        public string Description { get => Get<string>(); set => Set(value); }

        [JsonIgnore]
        [Description("Наименование")]
        public string DisplayName => $"Техника: {Name}";
    }
}
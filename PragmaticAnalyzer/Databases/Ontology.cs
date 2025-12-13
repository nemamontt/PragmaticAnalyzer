using PragmaticAnalyzer.Abstractions;
using PragmaticAnalyzer.Core;
using System.Collections.ObjectModel;

namespace PragmaticAnalyzer.Databases
{
    public interface IObjectOntology
    {
        string Name { get; set; }
        string Description { get; set; }
    }
    public class Ontology : ViewModelBase, IHasId, IObjectOntology
    {
        public Guid GuidId { get; set; } = Guid.NewGuid();
        public string Name { get => Get<string>(); set => Set(value); }
        public string Description { get => Get<string>(); set => Set(value); }
        public ObservableCollection<Entitie> Entities { get; set; }
    }

    public class Entitie : ViewModelBase, IHasId, IObjectOntology
    {
        public Guid GuidId { get; set; } = Guid.NewGuid();
        public string Name { get => Get<string>(); set => Set(value); }
        public string Description { get => Get<string>(); set => Set(value); }
    }
}
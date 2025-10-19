using PragmaticAnalyzer.Abstractions;
using PragmaticAnalyzer.Core;
using System.Collections.ObjectModel;

namespace PragmaticAnalyzer.Databases
{
    public class Outcomes
    {
        public ObservableCollection<Technology> Technologys { get; set; } = [];
        public ObservableCollection<Consequence> Consequences { get; set; } = [];
    }

    public class Technology : ViewModelBase, IDatabase
    {
        public Guid GuidId { get; } = Guid.NewGuid();
        public string SequenceNumber { get => Get<string>(); set => Set(value); }
        public string MethodName { get => Get<string>(); set => Set(value); }
        public string Description { get => Get<string>(); set => Set(value); }
        public string Usage { get => Get<string>(); set => Set(value); }
        public string Scale { get => Get<string>(); set => Set(value); }
        public string Horizont { get => Get<string>(); set => Set(value); }
        public string Level { get => Get<string>(); set => Set(value); }
        public string Necessity { get => Get<string>(); set => Set(value); }
        public string Experience { get => Get<string>(); set => Set(value); }
        public string Сharacteristic { get => Get<string>(); set => Set(value); }
        public string Effort { get => Get<string>(); set => Set(value); }
    }

    public class Consequence : ViewModelBase, IDatabase
    {
        public Guid GuidId { get; } = Guid.NewGuid();
        public string Number { get => Get<string>(); set => Set(value); }
        public string Name { get => Get<string>(); set => Set(value); }
        public string Damage { get => Get<string>(); set => Set(value); }
        public ObservableCollection<string> NameThreats { get; set; }
    }
}
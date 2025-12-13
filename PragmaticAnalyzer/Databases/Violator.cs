using PragmaticAnalyzer.Abstractions;
using PragmaticAnalyzer.Core;
using PragmaticAnalyzer.Enums;
using System.Text.Json.Serialization;

namespace PragmaticAnalyzer.Databases
{
    public class Violator : ViewModelBase, IHasId
    {
        public Guid GuidId { get; set; } = Guid.NewGuid(); //уникальный идентификатор
        public int SequenceNumber { get => Get<int>(); set => Set(value); } //порядковый номер
        public ViolatorSource Source { get => Get<ViolatorSource>(); set => Set(value); } //источник угрозы
        public ViolatorPotential Potential { get => Get<ViolatorPotential>(); set => Set(value); } //потенциал
        public string Target { get => Get<string>(); set => Set(value); } //цель атаки 
        public string UsingTools { get => Get<string>(); set => Set(value); } //используемые инструменты 
        public string PreviousAttacks { get => Get<string>(); set => Set(value); } //данные о предыдущих атаках 
        [JsonIgnore]
        public string DisplayedId => "VIO-" + SequenceNumber; //идентификатор для отображение в UI
    }
}
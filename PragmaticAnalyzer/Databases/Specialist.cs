using PragmaticAnalyzer.Abstractions;
using PragmaticAnalyzer.Core;
using System.Collections.ObjectModel;

namespace PragmaticAnalyzer.Databases
{
    public class Specialist : ViewModelBase, IDatabase
    {
        public Guid GuidId { get; } = Guid.NewGuid(); //уникальный идентификатор
        public string NameOrgan { get => Get<string>(); set => Set(value); } //наименование ОВУ 
        public string NameHighestOrgan { get => Get<string>(); set => Set(value); } //наименование вышестоящего ОВУ 
        public string NameSubordinateOrgan { get => Get<string>(); set => Set(value); } //наименование подчиненного ОВУ 
        public string StatusVulnerability { get => Get<string>(); set => Set(value); } //статус по выявленной уязвимости
        public string ActionsTaken { get => Get<string>(); set => Set(value); } //предпринятые действия
        public string NameSoftware { get => Get<string>(); set => Set(value); } //наименование СС СОПКА
        public ObservableCollection<string> NameInteractingOrgans { get; set; } = []; //наименование взаимодействующих ОВУ 
        public ObservableCollection<string> UsingMeasures { get; set; } = []; //список принятых мер защиты 
    }
}
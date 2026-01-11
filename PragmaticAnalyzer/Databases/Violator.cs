using PragmaticAnalyzer.Abstractions;
using PragmaticAnalyzer.Core;
using System.Collections.ObjectModel;

namespace PragmaticAnalyzer.Databases
{
    /// <summary>
    /// База данных "Нарушители"
    /// </summary>
    public class Violator : ViewModelBase, IHasId, IHasDescription
    {
        public Guid GuidId { get; set; } = Guid.NewGuid(); //уникальный идентификатор
        public string Description { get => Get<string>(); set => Set(value); } //описание
        public string GroupName { get => Get<string>(); set => Set(value); } //название группировки
        public string StateAffiliation { get => Get<string>(); set => Set(value); } //государственная принадлежность
        public string AlternateNames { get => Get<string>(); set => Set(value); } //второстепенные названия
        public string AttackTargets { get => Get<string>(); set => Set(value); } //объекты атаки
        public string KnownAttacks { get => Get<string>(); set => Set(value); } //известные атаки
        public string UsedTools { get => Get<string>(); set => Set(value); } //применяемые инструменты
        public string AttackObjectives { get => Get<string>(); set => Set(value); } //цели атаки
        public ObservableCollection<Tactics> TacticsUsed { get; set; } = []; //применяемые тактики
    }

    public class Tactics
    {
        public ObservableCollection<TacticEntry> Reconnaissance { get; set; } = [];  //разведка
        public ObservableCollection<TacticEntry> ResourceDevelopment { get; set; } = []; //разработка ресурсов
        public ObservableCollection<TacticEntry> InitialAccess { get; set; } = []; //первоначальный доступ
        public ObservableCollection<TacticEntry> Execution { get; set; } = []; //выполнение
        public ObservableCollection<TacticEntry> Persistence { get; set; } = []; //закрепление
        public ObservableCollection<TacticEntry> PrivilegeEscalation { get; set; } = []; //повышение привилегий
        public ObservableCollection<TacticEntry> DefenseEvasion { get; set; } = []; //обход защиты
        public ObservableCollection<TacticEntry> CredentialAccess { get; set; } = []; //доступ к учетным данным
        public ObservableCollection<TacticEntry> Discovery { get; set; } = []; //обнаружение
        public ObservableCollection<TacticEntry> LateralMovement { get; set; } = []; //боковое перемещение
        public ObservableCollection<TacticEntry> Collection { get; set; } = []; //сбор
        public ObservableCollection<TacticEntry> CommandAndControl { get; set; } = []; //управление и контроль
        public ObservableCollection<TacticEntry> Exfiltration { get; set; } = []; //эксфильтрация
        public ObservableCollection<TacticEntry> Impact { get; set; } = []; //воздействие
    }

    public class TacticEntry : ViewModelBase
    {
        public string Id { get => Get<string>(); set => Set(value); } //идентификатор
        public string Name { get => Get<string>(); set => Set(value); } //наименование
        public string Description { get => Get<string>(); set => Set(value); } //описание
    }
}
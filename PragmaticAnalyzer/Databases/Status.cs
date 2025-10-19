using PragmaticAnalyzer.Abstractions;

namespace PragmaticAnalyzer.Databases
{
    public class CurrentStatus : IDatabase
    {
        public Guid GuidId { get; } = Guid.NewGuid(); //уникальный идентификатор
        public string NameSoftware { get; set; }
        public string TimeAdmission {  get; set; }
        public string Dislocation { get; set; } 
        public string OperatingMode { get; set; }
        public string ImportanceCoefficient { get; set; }     
        public string Source { get; set; }
        public string AtributName { get; set; }
        public string AtributValue { get; set; }
        public string AtributRequiredValue { get; set; }
        public string ValueTeg {  get; set; }
        public List<Setting> Settings { get; set; }
    }

    public class ReferenceStatus : IDatabase
    {
        public Guid GuidId { get; } = Guid.NewGuid(); //уникальный идентификатор
        public string NameSoftware { get; set; } //Эталонное состояние
        public DateTime ArrivalTime { get; set; } //Время поступления ПД в систему
    }

    public class Setting
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<SettingParameter> SettingParameters { get; set; }
    }

    public class SettingParameter
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
    }
}
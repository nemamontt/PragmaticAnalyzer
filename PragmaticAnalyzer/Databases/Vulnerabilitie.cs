using PragmaticAnalyzer.Abstractions;
using PragmaticAnalyzer.Core;

namespace PragmaticAnalyzer.Databases
{
    public class Vulnerabilitie : ViewModelBase, IDatabase
    {
        public Guid GuidId { get; } = Guid.NewGuid(); //уникальный идентификатор
        public string Identifier { get => Get<string>(); set => Set(value); } //идентификатор
        public string Name { get => Get<string>(); set => Set(value); } //Наименование уязвимости
        public string Description { get => Get<string>(); set => Set(value); } //Описание уязвимости
        public string Vendor { get => Get<string>(); set => Set(value); } //Вендор ПО
        public string NameSoftware { get => Get<string>(); set => Set(value); } //Название ПО
        public string Version { get => Get<string>(); set => Set(value); } //Версия ПО
        public string Type { get => Get<string>(); set => Set(value); } //Тип ПО
        public string NameOperatingSystem { get => Get<string>(); set => Set(value); } //Наименование ОС и тип аппаратной платформы
        public string Class { get => Get<string>(); set => Set(value); } //Класс уязвимости
        public string Date { get => Get<string>(); set => Set(value); } //Дата выявления
        public string CvssTwo { get => Get<string>(); set => Set(value); } //CVSS 2.0
        public string CvssThree { get => Get<string>(); set => Set(value); } //CVSS 3.0
        public string DangerLevel { get => Get<string>(); set => Set(value); } //Уровень опасности уязвимости
        public string Measure { get => Get<string>(); set => Set(value); } //Возможные меры по устранению
        public string Exploit { get => Get<string>(); set => Set(value); } //Наличие эксплойта
        public string Information { get => Get<string>(); set => Set(value); } //Информация об устранении
        public string Links { get => Get<string>(); set => Set(value); } //Ссылки на источники
        public string OtherIdentifier { get => Get<string>(); set => Set(value); } //Идентификаторы других систем описаний уязвимости
        public string OtherInformation { get => Get<string>(); set => Set(value); } //Прочая информация
        public string Incident { get => Get<string>(); set => Set(value); } //Связь с инцидентами ИБ
        public string OperatingMethod { get => Get<string>(); set => Set(value); } //Способ эксплуатации
        public string EliminationMethod { get => Get<string>(); set => Set(value); } //Способ устранения
        public string DescriptionCwe { get => Get<string>(); set => Set(value); } //Описание ошибки CWE
        public string Cwe { get => Get<string>(); set => Set(value); } //Тип ошибки CWE
    }
}
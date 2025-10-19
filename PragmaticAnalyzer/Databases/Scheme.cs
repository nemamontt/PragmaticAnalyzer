using PragmaticAnalyzer.Abstractions;
using PragmaticAnalyzer.Core;
using System.Collections.ObjectModel;

namespace PragmaticAnalyzer.Databases
{
    public class Scheme : ViewModelBase
    {
        private int _recordCounter = 1;
        public string? Name
        {
            get => Get<string>();
            private set
            {
                Set(value ?? "Название не указано");
            }
        }
        public string? IndexPrefix
        {
            get => Get<string>();
            set
            {
                Set(value ?? "Запись");
            }
        }
        public ObservableCollection<string> CustomFieldNames { get; private set; }
        public ObservableCollection<Record> Records { get; private set; }

        public Scheme(string? name, string? indexPrefix, ObservableCollection<string> customFieldName)
        {
            Name = name;
            IndexPrefix = indexPrefix;
            CustomFieldNames = customFieldName ?? [];
            Records = [];
        }

        public void ChangeScheme(string? name, string? indexPrefix, ObservableCollection<string>? customFieldName)
        {
            if (name != null && name != string.Empty)
            {
                Name = name;
            }
            if (indexPrefix != null && indexPrefix != string.Empty)
            {
                IndexPrefix = indexPrefix;
                _recordCounter = 1;
                foreach (var record in Records)
                {
                    record.IndexValue = IndexPrefix + "-" + _recordCounter;
                    _recordCounter++;
                }
            }
            if (customFieldName != null) //удалить поля из Records
            {
                CustomFieldNames.Clear();
                foreach (var fieldName in customFieldName)
                {
                    CustomFieldNames.Add(fieldName);
                }
            }
        }

        public void AddRecord(string? description, Dictionary<string, string> customFields)
        {
            var record = new Record(IndexPrefix + "-" + _recordCounter)
            {
                Description = description,
            };
            _recordCounter++;
            foreach (var field in CustomFieldNames)
            {
                record[field] = customFields.TryGetValue(field, out var value) ? value : string.Empty;
            }
            Records.Add(record);
        }

        public bool ChangeRecord(Record record)
        {
            for (int i = 0; i < Records.Count; i++)
            {
                if (Records[i].GuidId == record.GuidId)
                {
                    Records[i].Description = record.Description;
                    Records[i].Fields = record.Fields;
                    return true;
                }
            }
            return false;
        }
    }

    public class Record : ViewModelBase, IDatabase
    {
        public Guid GuidId { get; } = Guid.NewGuid();
        public string IndexValue { get => Get<string>(); internal set => Set(value); }
        public string? Description
        {
            get => Get<string>();
            set
            {
                Set(value ?? "Отсутствует");
            }
        }
        public Dictionary<string, string> Fields { get => Get<Dictionary<string, string>>(); set => Set(value); }

        public Record(string indexValue)
        {
            IndexValue = indexValue;
            Description = string.Empty;
            Fields = [];
        }

        internal Record(Guid guidId, string indexValue)
        {
            GuidId = guidId;
            IndexValue = indexValue;
        }

        public string this[string fieldName]
        {
            get => Fields.TryGetValue(fieldName, out var value) ? value : string.Empty;
            set => Fields[fieldName] = value;
        }

        /*  public Dictionary<string, string> GetAllFields()
          {
              var result = new Dictionary<string, string>(Fields)
              {
                  ["GuidId"] = GuidId.ToString(),
                  ["Description"] = Description
              };
              return result;
          }*/
    }
}
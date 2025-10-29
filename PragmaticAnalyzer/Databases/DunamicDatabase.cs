using PragmaticAnalyzer.Abstractions;
using PragmaticAnalyzer.Core;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text.Json.Serialization;

namespace PragmaticAnalyzer.Databases
{
    public class DunamicDatabase : ViewModelBase
    {
        public int RecordCounter { get; set; }
        public Guid GuidId { get; } = Guid.NewGuid();
        public string Name { get => Get<string>(); private set => Set(value); }
        public string? IndexPrefix
        {
            get => Get<string>();
            set
            {
                Set(value ?? "Запись");
            }
        }
        public ObservableCollection<string> CustomFieldNames { get; private set; }
        [JsonIgnore]
        public ObservableCollection<DunamicRecord> Records { get; private set; }

        public DunamicDatabase(string name, string? indexPrefix, ObservableCollection<string> customFieldNames)
        {
            Name = name;
            IndexPrefix = indexPrefix;
            CustomFieldNames = customFieldNames ?? [];
            Records = [];
            Records.CollectionChanged += OnRecordsCollectionChanged;
        }

        private void OnRecordsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                int startIndex = e.OldStartingIndex;

                for (int i = startIndex; i < Records.Count; i++)
                {
                    var newIndex = i;
                    Records[i].IndexValue = $"{IndexPrefix}-{newIndex}";
                }

                RecordCounter--; 
            }
        }

        private void SyncRecordsWithSchema()
        {
            var validFieldNames = new HashSet<string>(CustomFieldNames, StringComparer.OrdinalIgnoreCase);

            foreach (var record in Records)
            {
                var keysToRemove = record.Fields.Keys.ToList()
                                         .Where(key => !validFieldNames.Contains(key))
                                         .ToList();

                foreach (var key in keysToRemove)
                {
                    record.Fields.Remove(key);
                }
            }
        }

        public void ChangeScheme(string? name, string? indexPrefix, ObservableCollection<string>? customFieldName)
        {
            if (name != null && name != string.Empty && Name != name)
            {
                Name = name;
            }
            if (indexPrefix != null && indexPrefix != string.Empty && IndexPrefix != indexPrefix)
            {
                IndexPrefix = indexPrefix;
                RecordCounter = 1;
                foreach (var record in Records)
                {
                    record.IndexValue = IndexPrefix + "-" + RecordCounter;
                    RecordCounter++;
                }
            }
            if (customFieldName != null)
            {
                CustomFieldNames.Clear();
                foreach (var customField in customFieldName)
                {
                    CustomFieldNames.Add(customField);
                }

                SyncRecordsWithSchema();
            }
        }

        public void AddRecord(string? description, Dictionary<string, string> customFields)
        {
            var record = new DunamicRecord(IndexPrefix + "-" + RecordCounter)
            {
                Description = description,
            };
            RecordCounter++;
            foreach (var field in CustomFieldNames)
            {
                record[field] = customFields.TryGetValue(field, out var value) ? value : string.Empty;
            }
            Records.Add(record);
        }

        public bool ChangeRecord(DunamicRecord record)
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

    public class DunamicRecord : ViewModelBase, IDatabase
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

        public DunamicRecord(string indexValue)
        {
            IndexValue = indexValue;
            Description = string.Empty;
            Fields = [];
        }

        internal DunamicRecord(Guid guidId, string indexValue)
        {
            GuidId = guidId;
            IndexValue = indexValue;
        }

        public string this[string fieldName]
        {
            get => Fields.TryGetValue(fieldName, out var value) ? value : string.Empty;
            set => Fields[fieldName] = value;
        }
    }
}
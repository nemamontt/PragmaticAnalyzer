using Aspose.Cells;
using PragmaticAnalyzer.Configs;
using PragmaticAnalyzer.Core;
using PragmaticAnalyzer.Databases;
using PragmaticAnalyzer.DTO;
using PragmaticAnalyzer.MVVM.Views.Main;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Xml.Linq;

namespace PragmaticAnalyzer.MVVM.ViewModel.Main
{
    public class CreatorViewModel : ViewModelBase
    {
        private readonly Func<object, string, DataType, Task> SaveDatabase;
        private readonly Action<string> DeleteDatabase;
        private DatabaseManagerView? _databaseManager;
        private RecordManagerView? _recordManager;
        private bool _expandedDatabases;
        private bool _expandedRecords;
        private bool _isAddDatabase;
        private bool _isAddRecord;

        public GridLength ColumnWidthDatabases { get => Get<GridLength>(); set => Set(value); }
        public GridLength ColumnWidthRecords { get => Get<GridLength>(); set => Set(value); }

        public ObservableCollection<DunamicDatabase> Databases { get; set; }
        public DunamicDatabase? SelectedDatabase { get => Get<DunamicDatabase>(); set => Set(value); }
        public string EnteredNameDatabase { get => Get<string>(); set => Set(value); }
        public string? EnteredNameField { get => Get<string>(); set => Set(value); }
        public string? EnteredNameIndexPrefix { get => Get<string>(); set => Set(value); }
        public ObservableCollection<string>? EnteredFieldsDatabase { get; set; }
        public string SelectedFieldDatabase { get => Get<string>(); set => Set(value); }
        public bool IsEnabledEnteredNameDatabase { get => Get<bool>(); set => Set(value); }

        public ObservableCollection<FieldInput>? FieldInputs { get => Get<ObservableCollection<FieldInput>>(); set => Set(value); }
        public DunamicRecord? SelectedRecord { get => Get<DunamicRecord>(); set => Set(value); }
        public string? EnteredDescriptionField { get => Get<string>(); set => Set(value); }

        public CreatorViewModel(ObservableCollection<DunamicDatabase> databases, Func<object, string, DataType, Task> saveDatabase, Action<string> deleteDatabase)
        {
            Databases = databases;
            SaveDatabase = saveDatabase;
            DeleteDatabase = deleteDatabase;
            _expandedDatabases = true;
            _expandedRecords = true;
            ColumnWidthDatabases = new GridLength(250);
            ColumnWidthRecords = new GridLength(250);
            IsEnabledEnteredNameDatabase = true;
        }

        public RelayCommand AddDatabaseCommand => GetCommand(o =>
        {
            EnteredNameDatabase = "unnamedDb";
            _isAddDatabase = true;
            EnteredFieldsDatabase = [];
            _databaseManager = new(this);
            _databaseManager.ShowDialog();
        });

        public RelayCommand ChangeDatabaseCommand => GetCommand(o =>
        {
            _isAddDatabase = false;
            IsEnabledEnteredNameDatabase = false;
            EnteredNameDatabase = SelectedDatabase.Name;
            EnteredFieldsDatabase = new(SelectedDatabase.CustomFieldNames);
            EnteredNameIndexPrefix = SelectedDatabase.IndexPrefix;
            _databaseManager = new(this);
            _databaseManager.ShowDialog();
        }, o => SelectedDatabase is not null);

        public RelayCommand DeleteDatabaseCommand => GetCommand(o =>
        {
            DeleteDatabase?.Invoke(SelectedDatabase.Name);
            Databases.Remove(SelectedDatabase);
        }, o => SelectedDatabase is not null);

        public RelayCommand AddFieldInDatabaseCommand => GetCommand(o =>
        {
            if (string.Equals(EnteredNameField, "Описание", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Запрещено использовать ключ = Описание (описание)");
                return;
            }
            foreach (var enteredField in EnteredFieldsDatabase)
            {
                if (enteredField == EnteredNameField)
                {
                    MessageBox.Show("Запрещено использовать повторяющийся ключ");
                    return;
                }
            }
            EnteredFieldsDatabase?.Add(EnteredNameField ?? string.Empty);
        }, o => EnteredNameField != string.Empty && EnteredNameField != null);

        public RelayCommand RemoveFieldInDatabaseCommand => GetCommand(o =>
        {
            EnteredFieldsDatabase?.Remove(SelectedFieldDatabase);
        }, o => SelectedFieldDatabase != null);

        public RelayCommand ApplyDatabaseManagerCommand => GetCommand(o =>
        {
            if (Databases.Any(item => string.Equals(item.Name, EnteredNameDatabase, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("База данных с таким именем уже существует");
                return;
            }
            else if (string.IsNullOrEmpty(EnteredNameDatabase))
            {
                MessageBox.Show("Поле с названием базы данных должно быть заполнено");
                return;
            }

            if (_isAddDatabase)
            {
                Databases.Add(new(EnteredNameDatabase, EnteredNameIndexPrefix, EnteredFieldsDatabase ?? []));
            }
            else
            {
                if (SelectedDatabase.Name != EnteredNameDatabase)
                {
                    DeleteDatabase?.Invoke(SelectedDatabase.Name);
                }
                SelectedDatabase.ChangeScheme(EnteredNameDatabase, EnteredNameIndexPrefix, EnteredFieldsDatabase ?? []);
                SaveDatabase?.Invoke(SelectedDatabase.Records, EnteredNameDatabase, DataType.DunamicDatabase);
            }
            CompleteDatabaseManager();
        });

        public RelayCommand CloseDatabaseManagerCommand => GetCommand(o =>
        {
            CompleteDatabaseManager();
        });

        public RelayCommand AddRecordCommand => GetCommand(o =>
        {
            FieldInputs = new ObservableCollection<FieldInput>(SelectedDatabase.CustomFieldNames.Select(name => new FieldInput(name)));
            _isAddRecord = true;
            _recordManager = new(this);
            _recordManager?.ShowDialog();
        }, o => SelectedDatabase is not null);

        public RelayCommand ChangeRecordCommand => GetCommand(o =>
        {
            FieldInputs = [];
            EnteredDescriptionField = SelectedRecord.Description;
            foreach (var fieldName in SelectedDatabase.CustomFieldNames)
            {
                var fieldInput = new FieldInput(fieldName)
                {
                    Value = SelectedRecord[fieldName]
                };
                FieldInputs.Add(fieldInput);
            }
            _isAddRecord = false;
            _recordManager = new(this);
            _recordManager.ShowDialog();
        }, o => SelectedDatabase is not null && SelectedRecord is not null);

        public RelayCommand DeleteRecordCommand => GetCommand(o =>
        {
            if (SelectedDatabase.Records.Count == 1)
            {
                DeleteDatabase?.Invoke(SelectedDatabase.Name);
                SelectedDatabase.Records.Remove(SelectedRecord);
            }
            else
            {
                SelectedDatabase.Records.Remove(SelectedRecord);
                SaveDatabase?.Invoke(SelectedDatabase.Records, SelectedDatabase.Name, DataType.DunamicDatabase);
            }

        }, o => SelectedDatabase is not null && SelectedRecord is not null);

        public RelayCommand ApplyRecordManagerCommand => GetCommand(o =>
        {
            var customFields = FieldInputs.ToDictionary(
                 input => input.Name,
                 input => input.Value ?? string.Empty
              );
            if (_isAddRecord)
            {
                SelectedDatabase.AddRecord(EnteredDescriptionField, customFields);
            }
            else
            {
                DunamicRecord record = new(SelectedRecord.GuidId, SelectedRecord.IndexValue)
                {
                    Description = EnteredDescriptionField,
                    Fields = customFields
                };
                SelectedDatabase.ChangeRecord(record);
            }
            SaveDatabase?.Invoke(SelectedDatabase.Records, SelectedDatabase.Name, DataType.DunamicDatabase);
            CompleteRecordManager();
            FieldInputs = null;
        });

        public RelayCommand CloseRecordManagerCommand => GetCommand(o =>
        {
            CompleteRecordManager();
        });

        public RelayCommand ZoomCommand => GetCommand(isSizeBd =>
        {
            bool flag = isSizeBd?.ToString().Equals("True", StringComparison.OrdinalIgnoreCase) == true;
            if (flag)
            {
                if (_expandedDatabases)
                {
                    ColumnWidthDatabases = new GridLength(0);
                    _expandedDatabases = false;
                }
                else
                {
                    ColumnWidthDatabases = new GridLength(250);
                    _expandedDatabases = true;
                }
            }
            else
            {
                if (_expandedRecords)
                {
                    ColumnWidthRecords = new GridLength(0);
                    _expandedRecords = false;
                }
                else
                {
                    ColumnWidthRecords = new GridLength(250);
                    _expandedRecords = true;
                }
            }
        });

        public void CompleteDatabaseManager()
        {
            EnteredNameDatabase = null;
            EnteredNameField = null;
            EnteredFieldsDatabase = null;
            EnteredNameIndexPrefix = null;
            SelectedDatabase = null;
            IsEnabledEnteredNameDatabase = true;
            _databaseManager?.Close();
        }

        public void CompleteRecordManager()
        {
            EnteredDescriptionField = null;
            SelectedRecord = null;
            FieldInputs = null;
            _recordManager?.Close();
        }
    }

    public class FieldInput(string name) : ViewModelBase
    {
        public string Name { get; } = name;
        public string Value { get => Get<string>(); set => Set(value); }
    }
}
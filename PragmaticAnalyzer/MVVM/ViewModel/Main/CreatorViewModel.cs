using CommunityToolkit.Mvvm.ComponentModel;
using PragmaticAnalyzer.Abstractions;
using PragmaticAnalyzer.Core;
using PragmaticAnalyzer.Databases;
using PragmaticAnalyzer.Extensions;
using PragmaticAnalyzer.MVVM.Views.Main;
using System.Collections.ObjectModel;
using System.Windows;

namespace PragmaticAnalyzer.MVVM.ViewModel.Main
{
    public class CreatorViewModel : ViewModelBase
    {
        private DatabaseManagerView? _databaseManager;
        private EntryManagerView? _recordManager;
        private bool _expandedDatabases { get; set; }
        private bool _expandedRecords { get; set; }
        private bool _isAddDatabase;
        private bool _isAddRecord;

        public GridLength ColumnWidthDatabases { get => Get<GridLength>(); set => Set(value); }
        public GridLength ColumnWidthRecords { get => Get<GridLength>(); set => Set(value); }

        public ObservableCollection<Scheme> Databases { get; set; } = [];
        public Scheme SelectedDatabase { get => Get<Scheme>(); set => Set(value); }
        public string? EnteredNameDatabase { get => Get<string>(); set => Set(value); }
        public string? EnteredNameField { get => Get<string>(); set => Set(value); }
        public string? EnteredNameIndexPrefix { get => Get<string>(); set => Set(value); }
        public ObservableCollection<string>? EnteredFieldsDatabase { get; set; }
        public string SelectedFieldDatabase { get => Get<string>(); set => Set(value); }

        public ObservableCollection<FieldInput>? FieldInputs { get => Get<ObservableCollection<FieldInput>>(); set => Set(value); }
        public Record SelectedRecord { get => Get<Record>(); set => Set(value); }
        public string? EnteredDescriptionField { get => Get<string>(); set => Set(value); }


        public CreatorViewModel()
        {
            _expandedDatabases = true;
            _expandedRecords = true;
            ColumnWidthDatabases = new GridLength(250);
            ColumnWidthRecords = new GridLength(250);
        }

        public RelayCommand AddDatabaseCommand => GetCommand(o =>
        {
            _isAddDatabase = true;
            EnteredFieldsDatabase = [];
            _databaseManager = new(this);
            _databaseManager.ShowDialog();
        });

        public RelayCommand ChangeDatabaseCommand => GetCommand(o =>
        {
            _isAddDatabase = false;
            EnteredNameDatabase = SelectedDatabase.Name;
            EnteredFieldsDatabase = new(SelectedDatabase.CustomFieldNames);
            _databaseManager = new(this);
            _databaseManager.ShowDialog();
        }, o => SelectedDatabase is not null);

        public RelayCommand DeleteDatabaseCommand => GetCommand(o =>
        {
            Databases.Remove(SelectedDatabase);
        }, o => SelectedDatabase is not null);

        public RelayCommand ApplyDatabaseManagerCommand => GetCommand(o =>
        {
            if (_isAddDatabase)
            {
                Databases.Add(new(EnteredNameDatabase, EnteredNameIndexPrefix, EnteredFieldsDatabase ?? []));
            }
            else
            {
                SelectedDatabase.ChangeScheme(EnteredNameDatabase, EnteredNameIndexPrefix, EnteredFieldsDatabase ?? []);
            }
            _databaseManager?.Close();
            ClearDatabaseManager();
        });

        public RelayCommand CloseDatabaseManagerCommand => GetCommand(o =>
        {
            _databaseManager?.Close();
        });

        public RelayCommand AddFieldInDatabaseCommand => GetCommand(o =>
        {
            foreach (var enteredField in EnteredFieldsDatabase)
            {
                if(EnteredNameField == "Описание" || enteredField == EnteredNameField)
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
            SelectedDatabase.Records.Remove(SelectedRecord);
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
                Record record = new(SelectedRecord.GuidId, SelectedRecord.IndexValue)
                {
                    Description = EnteredDescriptionField,
                    Fields = customFields
                };
                SelectedDatabase.ChangeRecord(record);
            }
            EnteredDescriptionField = null;
            _recordManager?.Close();
            FieldInputs = null;
        });

        public RelayCommand CloseRecordManagerCommand => GetCommand(o =>
        {
            _recordManager?.Close();
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

        public void ClearDatabaseManager()
        {
            EnteredNameDatabase = null;
            EnteredNameField = null;
            EnteredFieldsDatabase = null;
            EnteredNameIndexPrefix = null;
        }
    }

    public class FieldInput(string name) : ViewModelBase
    {
        public string Name { get; } = name;
        public string Value { get => Get<string>(); set => Set(value); }
    }
}
using PragmaticAnalyzer.Abstractions;
using PragmaticAnalyzer.Configs;
using PragmaticAnalyzer.Core;
using PragmaticAnalyzer.Databases;
using PragmaticAnalyzer.DTO;
using PragmaticAnalyzer.MVVM.Views.Main;
using PragmaticAnalyzer.Services;
using System.Collections.ObjectModel;

namespace PragmaticAnalyzer.MVVM.ViewModel.Main
{
    public class OntologyViewModel : ViewModelBase
    {
        private readonly IFileService _fileService;
        private readonly IDialogService _dialogService;
        private OntologyManagerView? _manager;
        public ObservableCollection<Ontology> Ontologys { get; set; }
        public IObjectOntology SelectedItem { get => Get<IObjectOntology>(); set => Set(value); }
        public string EnteredName { get => Get<string>(); set => Set(value); }
        public string EnteredDescription { get => Get<string>(); set => Set(value); }
        public bool IsAdd { get => Get<bool>(); set => Set(value); }
        public bool AddNewEntity { get => Get<bool>(); set => Set(value); }
        public bool IsEnabledCheckBox { get => Get<bool>(); set => Set(value); }

        public OntologyViewModel(ObservableCollection<Ontology> ontologys)
        {  
            Ontologys = ontologys;
            _fileService = new FileService();
            _dialogService = new DialogService();
            IsEnabledCheckBox = true;
        }

        public RelayCommand LoadCommand => GetCommand(async o =>
        {
            var path = _dialogService.OpenFileDialog(DialogService.JsonFilter);
            if (path is null) return;
            var ontology = await _fileService.LoadDTOAsync<Ontology>(path, DataType.Ontology);
            if (ontology is null) return;
            Ontologys.Add(ontology);
            await _fileService.SaveDTOAsync(Ontologys, DataType.Ontology, GlobalConfig.OntologyPath);
        });

        public RelayCommand AddCommand => GetCommand(o =>
        {
            IsEnabledCheckBox = true;
            IsAdd = true;
            _manager = new(this);
            _manager.ShowDialog();
        });

        public RelayCommand ChangeCommand => GetCommand(o =>
        {
            IsEnabledCheckBox = false;
            IsAdd = false;
            if (SelectedItem is Ontology)
            {
                EnteredName = SelectedItem.Name;
                EnteredDescription = SelectedItem.Description;
            }
            else if (SelectedItem is Entitie)
            {
                EnteredName = SelectedItem.Name;
                EnteredDescription = SelectedItem.Description;
            }
            _manager = new(this);
            _manager.ShowDialog();
        }, o => Ontologys.Count != 0);

        public RelayCommand DeleteCommand => GetCommand(async o =>
        {
            if (SelectedItem is Ontology ontology)
            {
                Ontologys.Remove(ontology);
            }
            else if (SelectedItem is Entitie entitie)
            {
                var parent = Ontologys.FirstOrDefault(o => o.Entities.Contains(entitie));
                parent?.Entities.Remove(entitie);
            }
            await _fileService.SaveDTOAsync(Ontologys, DataType.Ontology, GlobalConfig.OntologyPath);
        }, o => Ontologys.Count != 0);

        public RelayCommand ApplyCommand => GetCommand(async o =>
        {
            if (IsAdd)
            {
                if (SelectedItem is Ontology ontology && AddNewEntity)
                {
                    ontology.Entities ??= [];
                    ontology.Entities.Add(new()
                    {
                        Name = EnteredName,
                        Description = EnteredDescription
                    });
                }
                else
                {
                    Ontologys.Add(new()
                    {
                        Name = EnteredName,
                        Description = EnteredDescription
                    });
                }
            }
            else
            {
                SelectedItem.Name = EnteredName;
                SelectedItem.Description = EnteredDescription;
            }
            ResetUIManager();
            _manager?.Close();
            await _fileService.SaveDTOAsync(Ontologys, DataType.Ontology, GlobalConfig.OntologyPath);
        });

        public RelayCommand BackCommand => GetCommand(o =>
        {
            ResetUIManager();
            _manager?.Close();
        });

        public RelayCommand SelectedItemChangedCommand => GetCommand(selectedItem =>
        {
            SelectedItem = (IObjectOntology)selectedItem;
        });

        private void ResetUIManager()
        {
            EnteredName = string.Empty;
            EnteredDescription = string.Empty;
            AddNewEntity = false;   
        }
    }
}
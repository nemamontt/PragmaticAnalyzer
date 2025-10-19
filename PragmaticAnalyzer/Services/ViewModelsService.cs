using PragmaticAnalyzer.Abstractions;
using PragmaticAnalyzer.Configs;
using PragmaticAnalyzer.Core;
using PragmaticAnalyzer.Databases;
using PragmaticAnalyzer.DTO;
using PragmaticAnalyzer.Extensions;
using PragmaticAnalyzer.MVVM.ViewModel.Main;
using PragmaticAnalyzer.MVVM.ViewModel.Viewer;
using System.Collections.ObjectModel;
using System.IO;

namespace PragmaticAnalyzer.Services
{
    public class ViewModelsService : IViewModelsService
    {
        private readonly IFileService _fileService = new FileService();
        private readonly VulConfig _vulConfig = new();
        private readonly ThreatConfig _threatConfig = new();
        private readonly ExploitConfig _exploitConfig = new();
        private readonly LastUpdateConfig _lastUpdateConfig = new();
        private readonly ObservableCollection<AvailableDatabaseConfig> _availableDatabasesConfig = [];
        private readonly ObservableCollection<ModelConfig> _wordTwoVecConfig = [];
        private readonly ObservableCollection<ModelConfig> _fastTextVecConfig = [];
        public static IApiService ApiService => new ApiService();

        public MainViewModel MainVm { get; }
        public ThreatViewModel ThreatVm { get; }
        public VulnerabilitieViewModel VulnerabilitieVm { get; }
        public ExploitViewModel ExploitVm { get; set; }
        public OntologyViewModel OntologyVm { get; }
        public TacticViewModel TacticVm { get; }
        public ViolatorViewModel ViolatorVm { get; }
        public ProtectionMeasureViewModel ProtectionMeasureVm { get; }
        public SpecialistViewModel SpecialistVm { get; }
        public ReferenceStatusViewModel ReferenceStatusVm { get; }
        public CurrentStatusViewModel CurrentStatusVm { get; }
        public OutcomesViewModel OutcomeVm { get; }
        public SetViewModel SetVm { get; }
        public ViewerViewModel ViewerVm { get; }
        public SettingViewModel SettingVm { get; }
        public ConnectionViewModel ConnectionVm { get; }
        public InformationViewModel InformationVm { get; }
        public CreatorViewModel CreatorVm { get; }

        public ViewModelsService()
        {
            MainVm = new(this);
            SetVm = new(_lastUpdateConfig, this);
            ThreatVm = new([], SetVm.UpdateConfig, _threatConfig);
            VulnerabilitieVm = new([], SetVm.UpdateConfig, _vulConfig);
            ExploitVm = new([], SetVm.UpdateConfig, _exploitConfig);
            OntologyVm = new([]);
            TacticVm = new([], SetVm.UpdateConfig);
            ViolatorVm = new([], SetVm.UpdateConfig);
            ProtectionMeasureVm = new([], SetVm.UpdateConfig);
            SpecialistVm = new([], SetVm.UpdateConfig);
            ReferenceStatusVm = new([], SetVm.UpdateConfig);
            CurrentStatusVm = new([], SetVm.UpdateConfig);
            OutcomeVm = new(new(), SetVm.UpdateConfig);
            ViewerVm = new(this, _lastUpdateConfig, MainVm.OnSetCurrentView);
            ConnectionVm = new(this, ApiService, _availableDatabasesConfig);
            SettingVm = new(_wordTwoVecConfig, _fastTextVecConfig, ApiService);
            InformationVm = new(MainVm.OnSetCurrentView);
            CreatorVm = new();

            SetVm.UpdateThreatDb = ThreatVm.UpdateThreatDb;
            SetVm.UpdateExploitDb = ExploitVm.UpdateExploittDb;
            SetVm.UpdateVulDb = VulnerabilitieVm.UpdateVulDb;
        }

        public async Task InitializeAsync()
        {
            if (!Directory.Exists(GlobalConfig.DatabasePath))
            {
                Directory.CreateDirectory(GlobalConfig.DatabasePath);
            }
            if (!Directory.Exists(GlobalConfig.ExploitTextPath))
            {
                Directory.CreateDirectory(GlobalConfig.ExploitTextPath);
            }
            if (!Directory.Exists(GlobalConfig.ModelsPath))
            {
                Directory.CreateDirectory(GlobalConfig.ModelsPath);
            }
            if (!Directory.Exists(GlobalConfig.ConfigPath))
            {
                Directory.CreateDirectory(GlobalConfig.ConfigPath);
            }

            if (File.Exists(GlobalConfig.LastUpdateConfig))
            {
                _lastUpdateConfig.Update(await _fileService.LoadDTOAsync<LastUpdateConfig>(GlobalConfig.LastUpdateConfig, DataType.LastUpdateConfig) ?? new());
            }
            if (File.Exists(GlobalConfig.ExploitConfigPath))
            {
                _exploitConfig.Update(await _fileService.LoadDTOAsync<ExploitConfig>(GlobalConfig.ExploitConfigPath, DataType.ExploitConfig) ?? new());
            }
            if (File.Exists(GlobalConfig.ThreatConfigPath))
            {
                _threatConfig.Update(await _fileService.LoadDTOAsync<ThreatConfig>(GlobalConfig.ThreatConfigPath, DataType.ThreatConfig) ?? new());
            }
            if (File.Exists(GlobalConfig.VulConfigPath))
            {
                _vulConfig.Update(await _fileService.LoadDTOAsync<VulConfig>(GlobalConfig.VulConfigPath, DataType.VulConfig) ?? new());
            }
            if (File.Exists(GlobalConfig.WordTwoVecConfigPath))
            {
                var wordTwoVecConfigs = await _fileService.LoadDTOAsync<ObservableCollection<ModelConfig>>(GlobalConfig.WordTwoVecConfigPath, DataType.WordTwoVecConfig) ?? [];
                foreach (var config in wordTwoVecConfigs)
                {
                    if (config.Path != Path.Combine(GlobalConfig.ModelsPath, config.DisplayedName))
                    {
                        config.Path = Path.Combine(GlobalConfig.ModelsPath, config.DisplayedName);
                        await _fileService.SaveDTOAsync(wordTwoVecConfigs, DataType.WordTwoVecConfig, GlobalConfig.WordTwoVecConfigPath);
                    }
                }
                _wordTwoVecConfig.ReplaceAll(wordTwoVecConfigs);
            }
            if (File.Exists(GlobalConfig.FastTextConfigPath))
            {
                var fastTextConfigs = await _fileService.LoadDTOAsync<ObservableCollection<ModelConfig>>(GlobalConfig.FastTextConfigPath, DataType.FastTextConfig) ?? [];
                foreach (var config in fastTextConfigs)
                {
                    if (config.DisplayedName != Path.Combine(GlobalConfig.ModelsPath, config.DisplayedName))
                    {
                        config.Path = Path.Combine(GlobalConfig.ModelsPath, config.DisplayedName);
                        await _fileService.SaveDTOAsync(fastTextConfigs, DataType.FastTextConfig, GlobalConfig.FastTextConfigPath);
                    }
                }
                _fastTextVecConfig.ReplaceAll(fastTextConfigs);
            }

            //var vulDto2 = await _fileService.LoadDTOAsync<ObservableCollection<Vulnerabilitie>>(GlobalConfig.VulnerabilitiePath, DataType.Vulnerabilitie);
            var vulDto = await _fileService.LoadFileToPathAsync<DTO<ObservableCollection<Vulnerabilitie>>>(GlobalConfig.VulnerabilitiePath);
            if (vulDto != default)
            {
                foreach (var vul in vulDto.Value)
                {
                    VulnerabilitieVm.Vulnerabilities.Add(vul);
                }
                _availableDatabasesConfig.Add(new()
                {
                    Name = DataType.Vulnerabilitie,
                    FullName = GlobalConfig.VulnerabilitiePath,
                    SizeBytes = new FileInfo(GlobalConfig.VulnerabilitiePath).Length,
                    IsChecked = true,
                    LastModified = File.GetLastWriteTime(GlobalConfig.VulnerabilitiePath)
                });
                SetVm.UpdateConfig?.Invoke(vulDto.DateCreation.ToString("f"), DataType.Vulnerabilitie);
            }
            var threatDto = await _fileService.LoadFileToPathAsync<DTO<ObservableCollection<Threat>>>(GlobalConfig.ThreatPath);
            if (threatDto != default)
            {
                foreach (var threat in threatDto.Value)
                {
                    ThreatVm.Threats.Add(threat);
                }
                _availableDatabasesConfig.Add(new()
                {
                    Name = DataType.Threat,
                    FullName = GlobalConfig.ThreatConfigPath,
                    SizeBytes = new FileInfo(GlobalConfig.ThreatConfigPath).Length,
                    IsChecked = true,
                    LastModified = File.GetLastWriteTime(GlobalConfig.ThreatConfigPath)
                });
                SetVm.UpdateConfig?.Invoke(threatDto.DateCreation.ToString("f"), DataType.Threat);
            }
            var protectionMeasureDto = await _fileService.LoadFileToPathAsync<DTO<ObservableCollection<ProtectionMeasure>>>(GlobalConfig.ProtectionMeasurePath);
            if (protectionMeasureDto != default)
            {
                foreach (var protectionMeasure in protectionMeasureDto.Value)
                {
                    ProtectionMeasureVm.ProtectionMeasures.Add(protectionMeasure);
                }
                SetVm.UpdateConfig?.Invoke(protectionMeasureDto.DateCreation.ToString("f"), DataType.ProtectionMeasures);
            }
            var techniquesTacticDto = await _fileService.LoadFileToPathAsync<DTO<ObservableCollection<Tactic>>>(GlobalConfig.TacticPath);
            if (techniquesTacticDto != default)
            {
                foreach (var techniquesTactic in techniquesTacticDto.Value)
                {
                    TacticVm.Tactics.Add(techniquesTactic);
                }
                _availableDatabasesConfig.Add(new()
                {
                    Name = DataType.Tactic,
                    FullName = GlobalConfig.TacticPath,
                    SizeBytes = new FileInfo(GlobalConfig.TacticPath).Length,
                    IsChecked = true,
                    LastModified = File.GetLastWriteTime(GlobalConfig.TacticPath)
                });
                SetVm.UpdateConfig?.Invoke(techniquesTacticDto.DateCreation.ToString("f"), DataType.Tactic);
            }
            var exploitDto = await _fileService.LoadFileToPathAsync<DTO<ObservableCollection<Exploit>>>(GlobalConfig.ExploitPath);
            if (exploitDto != default)
            {
                foreach (var exploit in exploitDto.Value)
                {
                    ExploitVm.Exploits.Add(exploit);
                }
                _availableDatabasesConfig.Add(new()
                {
                    Name = DataType.Exploit,
                    FullName = GlobalConfig.ExploitPath,
                    SizeBytes = new FileInfo(GlobalConfig.ExploitPath).Length,
                    IsChecked = true,
                    LastModified = File.GetLastWriteTime(GlobalConfig.ExploitPath)
                });
                SetVm.UpdateConfig?.Invoke(exploitDto.DateCreation.ToString("f"), DataType.Exploit);
            }
            var outcomesDto = await _fileService.LoadFileToPathAsync<DTO<Outcomes>>(GlobalConfig.OutcomesPath);
            if (outcomesDto != default)
            {
                foreach (var technology in outcomesDto.Value.Technologys)
                {
                    OutcomeVm.Outcomes.Technologys.Add(technology);
                }
                foreach (var consequence in outcomesDto.Value.Consequences)
                {
                    OutcomeVm.Outcomes.Consequences.Add(consequence);
                }
                SetVm.UpdateConfig?.Invoke(outcomesDto.DateCreation.ToString("f"), DataType.Outcomes);
            }
            var specialistDto = await _fileService.LoadFileToPathAsync<DTO<ObservableCollection<Specialist>>>(GlobalConfig.SpecialistPath);
            if (specialistDto != default)
            {
                foreach (var specialist in specialistDto.Value)
                {
                    SpecialistVm.Specialists.Add(specialist);
                }
                SetVm.UpdateConfig?.Invoke(specialistDto.DateCreation.ToString("f"), DataType.Specialist);
            }
            var violatorDto = await _fileService.LoadFileToPathAsync<DTO<ObservableCollection<Violator>>>(GlobalConfig.ViolatorPath);
            if (violatorDto != default)
            {
                foreach (var violator in violatorDto.Value)
                {
                    ViolatorVm.Violators.Add(violator);
                }
                _availableDatabasesConfig.Add(new()
                {
                    Name = DataType.Violator,
                    FullName = GlobalConfig.ViolatorPath,
                    SizeBytes = new FileInfo(GlobalConfig.ViolatorPath).Length,
                    IsChecked = true,
                    LastModified = File.GetLastWriteTime(GlobalConfig.ViolatorPath)
                });
                SetVm.UpdateConfig?.Invoke(violatorDto.DateCreation.ToString("f"), DataType.Violator);
            }
            var currentStatusDto = await _fileService.LoadFileToPathAsync<DTO<ObservableCollection<CurrentStatus>>>(GlobalConfig.CurStatPath);
            if (currentStatusDto != default)
            {
                foreach (var currentStatus in currentStatusDto.Value)
                {
                    CurrentStatusVm.CurrentsStatus.Add(currentStatus);
                }
                SetVm.UpdateConfig?.Invoke(currentStatusDto.DateCreation.ToString("f"), DataType.CurrentStatus);
            }
            var referenceStatusDto = await _fileService.LoadFileToPathAsync<DTO<ObservableCollection<ReferenceStatus>>>(GlobalConfig.RefStatPath);
            if (referenceStatusDto != default)
            {
                foreach (var referenceStatus in referenceStatusDto.Value)
                {
                    ReferenceStatusVm.ReferencesStatus.Add(referenceStatus);
                }
                SetVm.UpdateConfig?.Invoke(referenceStatusDto.DateCreation.ToString("f"), DataType.ReferenceStatus);
            }
            var ontologyDto = await _fileService.LoadFileToPathAsync<DTO<ObservableCollection<Ontology>>>(GlobalConfig.OntologyPath);
            if (ontologyDto != default)
            {
                foreach (var ontology in ontologyDto.Value)
                {
                    OntologyVm.Ontologys.Add(ontology);
                }
            }

            SettingVm.NotifySelectedModels();

            await ApiService.StartServerAsync();
        }
    }
}
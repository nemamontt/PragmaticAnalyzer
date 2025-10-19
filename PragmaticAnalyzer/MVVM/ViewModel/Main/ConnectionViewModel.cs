using CommunityToolkit.Mvvm.Messaging;
using PragmaticAnalyzer.Abstractions;
using PragmaticAnalyzer.Configs;
using PragmaticAnalyzer.Core;
using PragmaticAnalyzer.Databases;
using PragmaticAnalyzer.Enums;
using PragmaticAnalyzer.Messages;
using PragmaticAnalyzer.MVVM.Views;
using PragmaticAnalyzer.WorkingServer.Matcher;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;

namespace PragmaticAnalyzer.MVVM.ViewModel.Main
{
    public class ConnectionViewModel : ViewModelBase,
        IRecipient<FastTextModelSelectedMessage>,
        IRecipient<Word2VecModelSelectedMessage>
    {
        private readonly IViewModelsService _viewModelsService;
        private readonly IApiService _apiService;
        private SettingSearchView _settingSearchView;
        private string? _wordTwoVecModelPath;
        private string? _fastTextModelPath;
        public ObservableCollection<AvailableDatabaseConfig> AvailableDatabasesConfig { get; private set; }
        public ObservableCollection<Report> Reports { get; private set; }
        public Report SelectedReport { get => Get<Report>(); set => Set(value); }
        public ObservableCollection<ProtectionMeasure> ProtectionMeasures { get; private set; }
        public ObservableCollection<Specialist> Specialists { get; private set; }
        public ObservableCollection<Consequence> Consequences { get; private set; }
        public ObservableCollection<Technology> Technologys { get; private set; }
        public ObservableCollection<Algorithm> Algorithms { get; private set; }
        public ProtectionMeasure SelectedProtectionMeasures { get => Get<ProtectionMeasure>(); set => Set(value); }
        public Specialist SelectedSpecialist { get => Get<Specialist>(); set => Set(value); }
        public Consequence SelectedConsequence { get => Get<Consequence>(); set => Set(value); }
        public Technology SelectedTechnology { get => Get<Technology>(); set => Set(value); }
        public Algorithm SelectedAlgorithm { get => Get<Algorithm>(); set => Set(value); }
        public string RequestText { get => Get<string>(); set => Set(value); }
        public bool Progress { get => Get<bool>(); set => Set(value); }
        public bool FilteringCvss { get => Get<bool>(); set => Set(value); }
        public Visibility ReportVisibility { get => Get<Visibility>(); set => Set(value); }

        public ConnectionViewModel(IViewModelsService viewModelsService, IApiService apiService, ObservableCollection<AvailableDatabaseConfig> availableDatabasesConfig)
        {
            WeakReferenceMessenger.Default.Register<FastTextModelSelectedMessage>(this);
            WeakReferenceMessenger.Default.Register<Word2VecModelSelectedMessage>(this);
            _viewModelsService = viewModelsService;
            _apiService = apiService;
            AvailableDatabasesConfig = availableDatabasesConfig;
            Reports = [];
            ProtectionMeasures = viewModelsService.ProtectionMeasureVm.ProtectionMeasures;
            Specialists = viewModelsService.SpecialistVm.Specialists;
            Consequences = viewModelsService.OutcomeVm.Outcomes.Consequences;
            Technologys = viewModelsService.OutcomeVm.Outcomes.Technologys;
            Algorithms = new(Enum.GetValues(typeof(Algorithm)).Cast<Algorithm>());
            RequestText = string.Empty;
            Progress = false;
            FilteringCvss = true;
            ReportVisibility = Visibility.Hidden;
        }

        public RelayCommand GenerateCommand => GetCommand(async o =>
        {
            Progress = true;

            string usedModel = string.Empty;
            switch (SelectedAlgorithm)
            {
                case Algorithm.TfIdf:
                    usedModel = string.Empty;
                    break;
                case Algorithm.FastText:
                    if (_fastTextModelPath is null)
                    {
                        Progress = false;
                        MessageBox.Show("Не выбрана модель");
                        return;
                    }
                    else
                    {
                        usedModel = _fastTextModelPath;
                    }
                    break;
                case Algorithm.WordTwoVec:
                    if (_wordTwoVecModelPath is null)
                    {
                        Progress = false;
                        MessageBox.Show("Не выбрана модель");
                        return;
                    }
                    else
                    {
                        usedModel = _wordTwoVecModelPath;
                    }
                    break;
            }

            RequestMatcher request = new("127.0.0.1", "5000", RequestText, SelectedAlgorithm, FilteringCvss, usedModel);

            var result = await _apiService.SendMatcherRequestAsync(request);
            if (!result.IsSuccess)
            {
                Progress = false;
                MessageBox.Show(result.ErrorMessage);
                return;
            }

            var reports = GetReports(result.Value);
            if (reports is null)
            {
                Progress = false;
                return;
            }

            Reports.Clear();
            foreach (var report in reports)
            {
                Reports.Add(report);
            }

            Progress = false;
            ReportVisibility = Visibility.Visible;
        }, o => RequestText != string.Empty && RequestText is not null && SelectedSpecialist is not null);

        public RelayCommand SaveReportCommand => GetCommand(async o =>
        {
            Progress = true;
            Progress = false;
        }, o => SelectedReport is not null);

        public RelayCommand SettingSearchCommand => GetCommand(o =>
        {
            _settingSearchView = new(this);
           _settingSearchView.ShowDialog();
        });

        public RelayCommand CloseSettingSearchCommand => GetCommand(o =>
        {
            _settingSearchView?.Close();
        });

        public RelayCommand ResetSearchCommand => GetCommand(o =>
        {
            RequestText = string.Empty;
            //SelectedVulnerabilitie = null;
        });//, o => SelectedVulnerabilitie is not null);

        public ObservableCollection<Report>? GetReports(ObservableCollection<ResponseMatcher> responseMatchers)
        {
            if (responseMatchers is null || responseMatchers.Count is 0)
            {
                return null;
            }

            var vulnerabilitiesDict = _viewModelsService.VulnerabilitieVm.Vulnerabilities.ToDictionary(v => v.GuidId);
            var threatsDict = _viewModelsService.ThreatVm.Threats.ToDictionary(t => t.GuidId);
            var violatorsDict = _viewModelsService.ViolatorVm.Violators.ToDictionary(v => v.GuidId);
            var tacticsDict = _viewModelsService.TacticVm.Tactics.ToDictionary(t => t.GuidId);
            var protectionMeasuresDict = _viewModelsService.ProtectionMeasureVm.ProtectionMeasures.ToDictionary(p => p.GuidId);
            var consequencesDict = _viewModelsService.OutcomeVm.Outcomes.Consequences.ToDictionary(c => c.GuidId);
            var technologysDict = _viewModelsService.OutcomeVm.Outcomes.Technologys.ToDictionary(t => t.GuidId);
            var exploitsDict = _viewModelsService.ExploitVm.Exploits.ToDictionary(e => e.GuidId);

            var results = new ObservableCollection<Report>();

            foreach (var responseMatcher in responseMatchers)
            {
                var report = new Report
                {
                    Coefficient = responseMatcher.Coefficient,
                    ProtectionMeasure = SelectedProtectionMeasures,
                    Specialist = SelectedSpecialist,
                    Consequence = SelectedConsequence,
                    Technology = SelectedTechnology
                };

                if (vulnerabilitiesDict.TryGetValue(responseMatcher.VulnerabilitieId, out var vulnerabilitie))
                    report.Vulnerabilitie = vulnerabilitie;

                if (threatsDict.TryGetValue(responseMatcher.ThreadId, out var threat))
                    report.Threat = threat;

                if (violatorsDict.TryGetValue(responseMatcher.ViolatorId, out var violator))
                    report.Violator = violator;

                if (tacticsDict.TryGetValue(responseMatcher.TacticId, out var tactic))
                    report.Tactic = tactic;

                if (protectionMeasuresDict.TryGetValue(responseMatcher.ProtectionMeasureId, out var protectionMeasure))
                    report.ProtectionMeasure = protectionMeasure;

                if (exploitsDict.TryGetValue(responseMatcher.ExploitId, out var exploit))
                    report.Exploit = exploit;

                results.Add(report);
            }

            return results;
        }

        public void Receive(FastTextModelSelectedMessage message)
        {
            _fastTextModelPath = message.ModelPath;
        }

        public void Receive(Word2VecModelSelectedMessage message)
        {
            _wordTwoVecModelPath = message.ModelPath;
        }
    }
    public class Report : ViewModelBase
    {
        public float Coefficient { get => Get<float>(); set => Set(value); }
        public Vulnerabilitie? Vulnerabilitie { get => Get<Vulnerabilitie>(); set => Set(value); }
        public Threat? Threat { get => Get<Threat>(); set => Set(value); }
        public Tactic? Tactic { get => Get<Tactic>(); set => Set(value); }
        public ProtectionMeasure? ProtectionMeasure { get => Get<ProtectionMeasure>(); set => Set(value); }
        public Technology? Technology { get => Get<Technology>(); set => Set(value); }
        public Consequence? Consequence { get => Get<Consequence>(); set => Set(value); }
        public Exploit? Exploit { get => Get<Exploit>(); set => Set(value); }
        public Specialist? Specialist { get => Get<Specialist>(); set => Set(value); }
        public Violator? Violator { get => Get<Violator>(); set => Set(value); }
        public ReferenceStatus? ReferenceStatus { get => Get<ReferenceStatus>(); set => Set(value); }
        public CurrentStatus? CurrentStatus { get => Get<CurrentStatus>(); set => Set(value); }
    }
}
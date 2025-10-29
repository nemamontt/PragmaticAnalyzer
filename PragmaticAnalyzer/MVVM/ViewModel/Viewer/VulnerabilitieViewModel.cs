using PragmaticAnalyzer.Abstractions;
using PragmaticAnalyzer.Configs;
using PragmaticAnalyzer.Core;
using PragmaticAnalyzer.Databases;
using PragmaticAnalyzer.DTO;
using PragmaticAnalyzer.MVVM.Model;
using PragmaticAnalyzer.Services;
using System.Collections.ObjectModel;
using System.Net.Http;

namespace PragmaticAnalyzer.MVVM.ViewModel.Viewer
{
    public class VulnerabilitieViewModel : ViewModelBase
    {
        private readonly VulnerabilitieModel _model;
        private readonly IFileService _fileService;
        private readonly Func<string, DataType, Task> UpdateConfig;
        private CancellationTokenSource? _updateCancellationTokenSource;
        public ObservableCollection<Vulnerabilitie> Vulnerabilities { get; }
        public Vulnerabilitie? SelectedVulnerabilitie { get => Get<Vulnerabilitie?>(); set => Set(value); }
        public string? Status { get => Get<string>(); set => Set(value); }
        public bool Progress { get => Get<bool>(); set => Set(value); }

        public VulnerabilitieViewModel(ObservableCollection<Vulnerabilitie> vulnerabilities, Func<string, DataType, Task> updateConfig, VulConfig vulConfig)
        {
            _fileService = new FileService();
            _model = new(vulConfig);
            _model.NotifyRequested += (msg) => Status += "\n" + msg;

            Vulnerabilities = vulnerabilities;
            UpdateConfig = updateConfig;
            Progress = false;
        }

        public RelayCommand UpdateCommand => GetCommand(async o =>
        {
            await UpdateVulDb();
        });//, o => _updateCancellationTokenSource is null);

        public RelayCommand CancelUpdateCommand => GetCommand(o =>
        {
            _updateCancellationTokenSource?.Cancel();
        });//, o => _updateCancellationTokenSource is not null);

        public async Task UpdateVulDb()
        {
            Progress = true;
            _updateCancellationTokenSource = new();
            try
            {
                Dictionary<string, string> forSelectingVulnerabilities = [];
                var newVulnerabilities = await _model.GetDatabase(_updateCancellationTokenSource.Token);
                if (newVulnerabilities is null) return;
                Vulnerabilities.Clear();
                foreach (var value in newVulnerabilities)
                {
                    Vulnerabilities.Add(value);
                    forSelectingVulnerabilities.Add(value.Identifier, value.Description);
                }

                await _fileService.SaveDTOAsync(Vulnerabilities, DataType.Vulnerabilitie, GlobalConfig.VulnerabilitiePath);
                UpdateConfig?.Invoke(DateTime.Now.ToString("f"), DataType.Vulnerabilitie);
            }
            catch (HttpRequestException httpEx)
            {
                Status += "\n" + httpEx.Message;
            }
            catch (OperationCanceledException ctEx)
            {
                Status += "\n" + ctEx.Message;
            }
            catch (Exception ex)
            {
                Status += "\n" + ex.Message;
            }
            finally
            {
                Progress = false;
                _updateCancellationTokenSource?.Dispose();
                _updateCancellationTokenSource = null;
            }
        }
    }
}
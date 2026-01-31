using PragmaticAnalyzer.Abstractions;
using PragmaticAnalyzer.Core;

namespace PragmaticAnalyzer.MVVM.ViewModel.Main
{
    public class MainViewModel : ViewModelBase
    {
        public IInfrastructureOrchestrator ViewModelsService { get => Get<IInfrastructureOrchestrator>(); private set => Set(value); }
        public object? CurrentView { get => Get<object>(); private set => Set(value); }

        public MainViewModel(IInfrastructureOrchestrator viewModelsService)
        {
            ViewModelsService = viewModelsService;
        }

        public RelayCommand SetCurrentViewCommand => GetCommand(vm => OnSetCurrentView(vm));

        public void OnSetCurrentView(object vm)
        {
            if (vm is not null)
            {
                CurrentView = vm;
            }
        }

        public async Task OnCompletionWork()
        {
            await ViewModelsService.CompletionWorkAsync();
        }
    }
}
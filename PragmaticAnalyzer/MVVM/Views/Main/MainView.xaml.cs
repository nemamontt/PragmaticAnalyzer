using PragmaticAnalyzer.MVVM.ViewModel.Main;
using PragmaticAnalyzer.Services;

namespace PragmaticAnalyzer.MVVM.Views.Main
{
    public partial class MainView
    {
        private MainViewModel _vm;
        public MainView(MainViewModel vm)
        {
            InitializeComponent();
            _vm = vm;
            DataContext = _vm;
            Closed += OnClosed;
        }

        private async void OnClosed(object sender, EventArgs e)
        {
            await _vm.OnCompletionWork();
            await ViewModelsService.ApiService.StopServerAsync();
        }
    }
}
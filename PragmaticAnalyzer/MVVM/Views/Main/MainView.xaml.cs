using PragmaticAnalyzer.Services;

namespace PragmaticAnalyzer.MVVM.Views.Main
{
    public partial class MainView
    {
        public MainView()
        {
            InitializeComponent();
            Closed += OnClosed;
        }

        private async void OnClosed(object sender, EventArgs e) => await ViewModelsService.ApiService.StopServerAsync();

    }
}
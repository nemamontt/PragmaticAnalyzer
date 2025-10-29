using PragmaticAnalyzer.MVVM.Views.Main;
using PragmaticAnalyzer.Services;
using System.Windows;

namespace PragmaticAnalyzer
{
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            var viewModelsService = new ViewModelsService();
            var mainVm = viewModelsService.MainVm;

            var mainView = new MainView(mainVm);
            mainView.Show();

            await viewModelsService.InitializeAsync();  

            base.OnStartup(e);
        }
    }
}
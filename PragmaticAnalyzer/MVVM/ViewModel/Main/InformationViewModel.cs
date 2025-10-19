using PragmaticAnalyzer.Core;
using System.Diagnostics;
using System.IO;
using PragmaticAnalyzer.MVVM.ViewModel.AlgorithmInformation;

namespace PragmaticAnalyzer.MVVM.ViewModel.Main
{
    public class InformationViewModel : ViewModelBase
    {
        private readonly Action<object> SetCurrentView;
        public FasttextInformationViewModel FasttextInformationVm {  get; set; }
        public WordTwoVectInformationViewModel WordTwoVectInformationVm { get; set; }
        public TfIdfInformationViewModel TfIdfInformationVm { get; set; }

        public InformationViewModel(Action<object> setCurrentView)
        {
            SetCurrentView = setCurrentView;
            FasttextInformationVm = new();
            WordTwoVectInformationVm = new();
            TfIdfInformationVm = new();
        }

        public RelayCommand ShowInformationCommand => GetCommand(o =>
        {
            SetCurrentView?.Invoke(o);
        });
    }
}
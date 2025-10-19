using PragmaticAnalyzer.MVVM.ViewModel.Main;

namespace PragmaticAnalyzer.MVVM.Views.Main
{

    public partial class EntryManagerView
    {
        public EntryManagerView(CreatorViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
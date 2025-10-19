using PragmaticAnalyzer.Core;

namespace PragmaticAnalyzer.Configs
{
    public class ThreatConfig : ViewModelBase
    {
        public string ParsingUrl {  get => Get<string>(); set => Set(value); }

        public ThreatConfig()
        {
            ParsingUrl = "https://bdu.fstec.ru/files/documents/thrlist.xlsx";
        }

        public void Update(ThreatConfig newThreatConfig)
        {
            if (newThreatConfig is null) return;

            ParsingUrl = newThreatConfig.ParsingUrl;
        }
    }
}
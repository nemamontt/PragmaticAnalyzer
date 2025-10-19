using PragmaticAnalyzer.Core;

namespace PragmaticAnalyzer.Configs
{
    public class VulConfig : ViewModelBase
    {
        public string UrlFstec { get => Get<string>(); set => Set(value); }
        public string UrlNvd { get => Get<string>(); set => Set(value); }
        public string UrlJvn { get => Get<string>(); set => Set(value); }
        public string ApiKeyNvd { get => Get<string>(); set => Set(value); }

        public VulConfig()
        {
            UrlFstec = "https://bdu.fstec.ru/files/documents/vullist.xlsx";
            UrlNvd = "";
            UrlJvn = "";
            ApiKeyNvd = "";
        }

        public void Update(VulConfig newVulConfig)
        {
            if (newVulConfig is null) return;

            UrlFstec = newVulConfig.UrlFstec;
            UrlNvd = newVulConfig.UrlNvd;
            UrlJvn = newVulConfig.UrlJvn;
            ApiKeyNvd = newVulConfig.ApiKeyNvd;
        }
    }
}
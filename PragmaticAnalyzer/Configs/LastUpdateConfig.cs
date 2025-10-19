using PragmaticAnalyzer.Core;

namespace PragmaticAnalyzer.Configs
{
    //
    //Объект, который хранит даты последнего обновления баз данных, сохраняется в формате .json
    //
    public class LastUpdateConfig : ViewModelBase
    {
        public string Vulnerabilitie { get => Get<string>(); set => Set(value); }
        public string Threat { get => Get<string>(); set => Set(value); }
        public string Tactic { get => Get<string>(); set => Set(value); }
        public string ProtectionMeasure { get => Get<string>(); set => Set(value); }
        public string Outcomes { get => Get<string>(); set => Set(value); }
        public string Exploit { get => Get<string>(); set => Set(value); }
        public string Violator { get => Get<string>(); set => Set(value); }
        public string Specialist { get => Get<string>(); set => Set(value); }
        public string RefStatus { get => Get<string>(); set => Set(value); }
        public string CurStatus { get => Get<string>(); set => Set(value); }

        public void Update(LastUpdateConfig newLastUpdateConfig)
        {
            if (newLastUpdateConfig is null) return;

            Vulnerabilitie = newLastUpdateConfig.Vulnerabilitie;
            Threat = newLastUpdateConfig.Threat;
            Tactic = newLastUpdateConfig.Tactic;
            ProtectionMeasure = newLastUpdateConfig.ProtectionMeasure;
            Outcomes = newLastUpdateConfig.Outcomes;
            Exploit = newLastUpdateConfig.Exploit;
            Violator = newLastUpdateConfig.Violator;
            Specialist = newLastUpdateConfig.Specialist;
            RefStatus = newLastUpdateConfig.RefStatus;
            CurStatus = newLastUpdateConfig.CurStatus;
        }
    }
}
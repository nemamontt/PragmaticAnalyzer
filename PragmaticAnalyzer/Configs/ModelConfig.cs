using PragmaticAnalyzer.Core;
using PragmaticAnalyzer.Enums;
using System.Text.Json.Serialization;

namespace PragmaticAnalyzer.Configs
{
    //
    //Объект, который хранит информацию о сохраненной модели, сохраняется в формате .json
    //
    public class ModelConfig : ViewModelBase
    {
        public string Path { get => Get<string>(); set => Set(value); }
        public Algorithm Algorithm { get => Get<Algorithm>(); set => Set(value); }
        public bool IsTrained { get => Get<bool>(); set => Set(value); }
        public bool IsUsed { get => Get<bool>(); set => Set(value); }
        [JsonIgnore]
        public string DisplayedName => System.IO.Path.GetFileName(Path);
    } 
}
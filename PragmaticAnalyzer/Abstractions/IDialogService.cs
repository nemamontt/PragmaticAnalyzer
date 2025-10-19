namespace PragmaticAnalyzer.Abstractions
{
    /// <summary>
    /// Интерфейс для сервиса, который работает с проводником Windows.
    /// </summary>
    public interface IDialogService
    {
        string? OpenFileDialog(string? filter = null);
        string? SaveFileDialog(string defaultFileName, string filter);
    }
}

namespace PragmaticAnalyzer.Abstractions
{
    public interface IMessageService
    {
        void ShowInformation(string message, string? title = null);
        void ShowError(string message, string? title = null);
    }
}

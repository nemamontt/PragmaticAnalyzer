using Microsoft.Win32;
using PragmaticAnalyzer.Abstractions;

namespace PragmaticAnalyzer.Services
{
    public class DialogService
    {
        public const string ExcelFilter = "Excel files (*.xlsx)|*.xlsx";
        public const string WordFilter = "Word files (*.docx)|*.docx";
        public const string JsonFilter = "Json files (*.json)|*.json";
        public const string ModelFilter = "Model files (*.bin)|*.bin";

        public static string? OpenFileDialog(string? filter = null)
        {
            var dialog = new OpenFileDialog
            {
                Filter = filter
            };
            return dialog.ShowDialog() is true ? dialog.FileName : null;
        }

        public static string? SaveFileDialog(string defaultFileName, string filter)
        {
            var dialog = new SaveFileDialog
            {
                Filter = filter,
                FileName = defaultFileName
            };
            return dialog.ShowDialog() is true ? dialog.FileName : null;
        }
    }
}
using PragmaticAnalyzer.Abstractions;
using PragmaticAnalyzer.Configs;
using PragmaticAnalyzer.DTO;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace PragmaticAnalyzer.Services
{
    public class FileService : IFileService
    {
        private readonly JsonSerializerOptions saveOption;

        public FileService()
        {
            saveOption = new()
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
        }

        public async Task<T?> LoadDTOAsync<T>(string? path, DataType type, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();

            if (!CheckPath(path) || !File.Exists(path))
                return default;

            try
            {
                var json = await File.ReadAllTextAsync(path, ct).ConfigureAwait(false);
                var value = JsonSerializer.Deserialize<DTO<T>>(json);
                if (value is not null && value.DtoType == type)
                    return value.Value;
                return default;
            }
            catch
            {
                return default;
            }
        }

        public async Task<T?> LoadFileToPathAsync<T>(string? path, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();

            if (!CheckPath(path) || !File.Exists(path))
                return default;

            try
            {
                var json = await File.ReadAllTextAsync(path, ct).ConfigureAwait(false);
                return JsonSerializer.Deserialize<T>(json);
            }
            catch
            {
                return default;
            }
        }

        public async Task<T?> LoadJsonAsync<T>(string? json, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();

            try
            {
                if (string.IsNullOrEmpty(json))
                {
                    return default;
                }
                return await Task.Run(() => JsonSerializer.Deserialize<T>(json), ct);
            }
            catch
            {
                return default;
            }
        }

        public async Task<bool> SaveDTOAsync<T>(T value, DataType type, string path, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();

            if (!CheckPath(path))
                return false;

            try
            {
                DTO<T> dto = new()
                {
                    DtoType = type,
                    Value = value,
                    DateCreation = DateTime.Now
                };
                var json = JsonSerializer.Serialize(dto, saveOption);
                await File.WriteAllTextAsync(path, json, ct).ConfigureAwait(false);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> SaveFileAsync(object value, string path, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();

            if (!CheckPath(path))
                return false;

            try
            {
                var json = JsonSerializer.Serialize(value, saveOption);
                await File.WriteAllTextAsync(path, json, ct).ConfigureAwait(false);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool CheckPath(string? path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;

            try
            {
                foreach (char c in Path.GetInvalidPathChars())
                {
                    if (path.Contains(c))
                        return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
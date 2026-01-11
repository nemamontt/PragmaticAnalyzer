using PragmaticAnalyzer.Abstractions;
using PragmaticAnalyzer.Configs;
using PragmaticAnalyzer.WorkingServer.Core;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;

namespace PragmaticAnalyzer.Services
{
    public class ApiService : IApiService
    {
        private Process? _translatorProcess;
        private readonly int _port = 5001;
        public bool IsRunningTranslator => _translatorProcess?.HasExited == false;
        private readonly HttpClient _httpClient;
        private readonly IFileService _fileService;

        public ApiService()
        {
            var handler = new SocketsHttpHandler
            {
                ConnectTimeout = TimeSpan.FromSeconds(250),
            };
            _httpClient = new(handler);
            _fileService = new FileService();
            /*  _serverProcess = new Process
              {
                  StartInfo = new ProcessStartInfo
                  {
                      FileName = GlobalConfig.MatcherPath,
                      UseShellExecute = false,
                      CreateNoWindow = true,
                      RedirectStandardOutput = false,
                      RedirectStandardError = false
                  }
              };*/
        }

        public void StartServer()
        {
            if (IsRunningTranslator)
            {
                return;
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = GlobalConfig.TranslatorPath,
                Arguments = $"--model \"{GlobalConfig.TranslatorYandexModelPath}\" --port {_port} --contextsize 2048",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = false,
                RedirectStandardError = false
            };

            _translatorProcess = new Process { StartInfo = startInfo };
             _translatorProcess.Start();
        }

        public void StopServer()
        {
            try
            {
                var processes = Process.GetProcessesByName("koboldcpp");
                foreach (var process in processes)
                {

                    if (!process.HasExited)
                    {
                        process.Kill();
                        process.WaitForExit(3000);
                    }
                    process.Dispose();
                }
            }
            catch (Exception ex) { }
            finally
            {
                _translatorProcess?.Dispose();
                _translatorProcess = null;
            }
        }

        public async Task<Result<T>> SendRequestAsync<T>(IRequest request, CancellationToken ct = default)
        {
            try
            {
                var responseHttp = await _httpClient.PostAsync(request.Url, request.Content, ct);

                if (!responseHttp.IsSuccessStatusCode)
                {
                    var errorContent = await responseHttp.Content.ReadAsStringAsync();
                    return Result<T>.Failure($"SОшибка сервера: {responseHttp.StatusCode}. Детали: {errorContent}");
                }
                var json = await responseHttp.Content.ReadAsStringAsync();
                var data = await _fileService.LoadJsonAsync<T>(json);
                return Result<T>.Success(data);
            }
            catch (HttpRequestException ex)
            {
                return Result<T>.Failure($"Сетевая ошибка: {ex.Message}");
            }
            catch (JsonException ex)
            {
                return Result<T>.Failure($"Ошибка парсинга JSON: {ex.Message}");
            }
            catch (Exception ex)
            {
                return Result<T>.Failure($"Неизвестная ошибка: {ex.Message}");
            }
        }
    }
}
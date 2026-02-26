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
        private Process? _koboldcppProcess; // процесс для сервера, который развертывает локальную модель
        private Process? _matcherProcess; // процесс для сервера работы с моделями
        public bool IsRunningKoboldcpp => _koboldcppProcess?.HasExited == false; // если процесс для сервера , который развертывает локальную модель активен true
        public bool IsRunningMatcher => _matcherProcess?.HasExited == false; // если процесс для сервера работы с моделями активен true
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
        }

        public void StartServer()
        {
 /*           if (!IsRunningKoboldcpp)
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = GlobalConfig.TranslatorPath,
                    Arguments = $"--model \"{GlobalConfig.TranslatorYandexModelPath}\" --port {GlobalConfig.TranslatorPort} --contextsize 2048",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = false,
                    RedirectStandardError = false
                };

                _koboldcppProcess = new Process { StartInfo = startInfo };
                _koboldcppProcess.Start();
            }
            else
            {
                return;
            }*/

          /*  if (!IsRunningMatcher)
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = GlobalConfig.MatcherPath,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = false,
                    RedirectStandardError = false
                };

                _matcherProcess = new Process { StartInfo = startInfo };
                 _matcherProcess.Start();
            }
            else
            {
                return;
            }*/
        } // запуск серверов

        public void StopServer()
        {
          /*  try
            {
                var translatorProcesses = Process.GetProcessesByName("koboldcpp");
                var matcherProcesses = Process.GetProcessesByName("matcher.exe");
                var result = translatorProcesses.Union(matcherProcesses).ToArray();

                foreach (var process in result)
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
                _koboldcppProcess?.Dispose();
                _matcherProcess?.Dispose();
                _koboldcppProcess = null;
                _matcherProcess = null;
            }*/
        } // остановка серверов

        public async Task<Result<T>> SendRequestAsync<T>(IRequest request, CancellationToken ct = default, int delay = 0)
        {
            try
            {
                ct.ThrowIfCancellationRequested();
                await Task.Delay(delay, ct).ConfigureAwait(false);

                ct.ThrowIfCancellationRequested();
                var responseHttp = await _httpClient.PostAsync(request.Url, request.Content, ct);

                if (!responseHttp.IsSuccessStatusCode)
                {
                    var errorContent = await responseHttp.Content.ReadAsStringAsync(ct);
                    return Result<T>.Failure($"SОшибка сервера: {responseHttp.StatusCode}. Детали: {errorContent}");
                }
                var json = await responseHttp.Content.ReadAsStringAsync(ct);
                var data = await _fileService.LoadJsonAsync<T>(json, ct);
                return Result<T>.Success(data);
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Операция была отменена");
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
    } // сервис для работы с сервером (внешними ресурсами), управляет процессами и проксирует запросы
}
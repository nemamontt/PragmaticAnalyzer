using PragmaticAnalyzer.Abstractions;
using PragmaticAnalyzer.Configs;
using PragmaticAnalyzer.WorkingServer.Core;
using PragmaticAnalyzer.WorkingServer.Matcher;
using PragmaticAnalyzer.WorkingServer.Retrain;
using PragmaticAnalyzer.WorkingServer.Train;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;

namespace PragmaticAnalyzer.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IFileService _fileService;
        private readonly Process _serverProcess;

        public ApiService()
        {
            var handler = new SocketsHttpHandler
            {
                ConnectTimeout = TimeSpan.FromSeconds(250),
            };
            _httpClient = new(handler);
            _fileService = new FileService();

            _serverProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = GlobalConfig.MatcherPath,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = false,
                    RedirectStandardError = false
                }
            };
        }

        public async Task StartServerAsync()
        {
            if(IsProcessAlive())
            {
                return;
            }
            _serverProcess.Start();
            await Task.Delay(5000);
        }

        public async Task StopServerAsync()
        {
/*
            if (!IsProcessAlive())
            {
                return;
            }*/

            await _httpClient.PostAsync("http://127.0.0.1:5000/Shutdown", new StringContent(""));
            await Task.Delay(1000);
            if (!_serverProcess.HasExited)
            {
                _serverProcess.Kill();
            }
            await _serverProcess.WaitForExitAsync();
            _serverProcess.Dispose();
        }

        public async Task<Result<ObservableCollection<ResponseMatcher>>> SendMatcherRequestAsync(RequestMatcher request)
        {
            try
            {
                var responseHttp = await _httpClient.PostAsync(request.Url, request.Content);
                if (!responseHttp.IsSuccessStatusCode)
                {            
                    var errorContent = await responseHttp.Content.ReadAsStringAsync();
                    return Result<ObservableCollection<ResponseMatcher>>.Failure($"Ошибка сервера: {responseHttp.StatusCode}. Детали: {errorContent}");
                }
                var json = await responseHttp.Content.ReadAsStringAsync();
                var data = await _fileService.LoadJsonAsync<ObservableCollection<ResponseMatcher>>(json) ?? throw new Exception("Входной текст не распознан");
                return Result<ObservableCollection<ResponseMatcher>>.Success(data);
            }
            catch (HttpRequestException ex)
            {
                return Result<ObservableCollection<ResponseMatcher>>.Failure($"Сетевая ошибка: {ex.Message}");
            }
            catch (JsonException ex)
            {
                return Result<ObservableCollection<ResponseMatcher>>.Failure($"Ошибка парсинга JSON: {ex.Message}");
            }
            catch (Exception ex)
            {
                return Result<ObservableCollection<ResponseMatcher>>.Failure($"Неизвестная ошибка: {ex.Message}");
            }
        }

        public async Task<Result<ResponseTrain>> SendTrainRequestAsync(RequestTrain request)
        {
            try
            {
                var responseHttp = await _httpClient.PostAsync(request.Url, request.Content);

                if (!responseHttp.IsSuccessStatusCode)
                {
                    var errorContent = await responseHttp.Content.ReadAsStringAsync();
                    return Result<ResponseTrain>.Failure($"SОшибка сервера: {responseHttp.StatusCode}. Детали: {errorContent}");
                }
                var json = await responseHttp.Content.ReadAsStringAsync();
                var data = await _fileService.LoadJsonAsync<ResponseTrain>(json);
                return Result<ResponseTrain>.Success(data);
            }
            catch (HttpRequestException ex)
            {
                return Result<ResponseTrain>.Failure($"Сетевая ошибка: {ex.Message}");
            }
            catch (JsonException ex)
            {
                return Result<ResponseTrain>.Failure($"Ошибка парсинга JSON: {ex.Message}");
            }
            catch (Exception ex)
            {
                return Result<ResponseTrain>.Failure($"Неизвестная ошибка: {ex.Message}");
            }
        }

        public async Task<Result<T>> SendRequestAsync<T>(IRequest request)
        {
            try
            {
                var responseHttp = await _httpClient.PostAsync(request.Url, request.Content);

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

        private bool IsProcessAlive()
        {
            try
            {
                return !_serverProcess.HasExited;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }
    }
}
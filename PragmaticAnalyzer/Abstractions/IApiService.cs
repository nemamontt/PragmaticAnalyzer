using PragmaticAnalyzer.WorkingServer.Core;

namespace PragmaticAnalyzer.Abstractions
{
    /// <summary>
    /// Интерфейс для сервиса взаимодействия с серверами
    /// </summary>
    public interface IApiService
    {
        void StartServer();
        void StopServer();
        Task<Result<T>> SendRequestAsync<T>(IRequest request, CancellationToken ct = default, int delay = 0);
    }
}
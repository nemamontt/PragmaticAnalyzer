using PragmaticAnalyzer.WorkingServer.Core;

namespace PragmaticAnalyzer.Abstractions
{
    public interface IApiService
    {
        void StartServer();
        Task StopServerAsync();
        Task<Result<T>> SendRequestAsync<T>(IRequest request);
    }
}
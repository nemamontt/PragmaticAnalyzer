using PragmaticAnalyzer.WorkingServer.Core;

namespace PragmaticAnalyzer.Abstractions
{
    public interface IApiService
    {
        Task StartServerAsync();
        Task StopServerAsync();
        Task<Result<T>> SendRequestAsync<T>(IRequest request);
    }
}
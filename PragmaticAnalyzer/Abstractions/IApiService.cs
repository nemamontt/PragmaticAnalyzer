using PragmaticAnalyzer.WorkingServer.Core;
using PragmaticAnalyzer.WorkingServer.Matcher;
using PragmaticAnalyzer.WorkingServer.Train;
using System.Collections.ObjectModel;

namespace PragmaticAnalyzer.Abstractions
{
    public interface IApiService
    {
        Task StartServerAsync();
        Task StopServerAsync();
        Task<Result<ObservableCollection<ResponseMatcher>>> SendMatcherRequestAsync(RequestMatcher request);
        Task<Result<ResponseTrain>> SendTrainRequestAsync(RequestTrain request);
    }
}
using System.Net.Http;

namespace PragmaticAnalyzer.Abstractions
{
    public interface IRequest
    {
        string Url { get; }
        StringContent? Content { get; }
    }
}

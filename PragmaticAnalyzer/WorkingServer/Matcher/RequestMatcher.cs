using PragmaticAnalyzer.Enums;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace PragmaticAnalyzer.WorkingServer.Matcher
{
    public class RequestMatcher
    {
        public string Url { get; private set; }
        public StringContent? Content { get; private set; }

        public RequestMatcher(string ip, string port, string textDescription, Algorithm algorithm, bool filteringCvss, string? usedModel)
        {
            var payload = new
            {
                TextDescription = textDescription,
                ModelName = algorithm.ToString(),
                FilteringCvss = filteringCvss,
                UsedModel = usedModel
            };
            string json = JsonSerializer.Serialize(payload);
            Url = $"http://{ip}:{port}/Matcher";
            Content = new(json, Encoding.UTF8, "application/json");
        }
    }
}

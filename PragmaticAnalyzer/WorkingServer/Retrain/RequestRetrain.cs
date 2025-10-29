using PragmaticAnalyzer.Abstractions;
using PragmaticAnalyzer.Enums;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace PragmaticAnalyzer.WorkingServer.Retrain
{
    public class RequestRetrain : IRequest
    {
        public string Url { get; private set; }
        public StringContent? Content { get; private set; }

        public RequestRetrain(string ip, string port, string modelPath, Algorithm nameAlgorithm)
        {
            var payload = new
            {
                ModelPath = modelPath,
                NameAlgorithm = nameAlgorithm.ToString()
            };
            string json = JsonSerializer.Serialize(payload);
            Url = $"http://{ip}:{port}/Retrain";
            Content = new(json, Encoding.UTF8, "application/json");
        }
    }
}
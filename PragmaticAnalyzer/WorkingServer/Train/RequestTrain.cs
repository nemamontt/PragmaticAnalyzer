using PragmaticAnalyzer.Enums;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace PragmaticAnalyzer.WorkingServer.Train
{
    public class RequestTrain
    {
        public string Url { get; private set; }
        public StringContent? Content { get; private set; }

        public RequestTrain(string ip, string port, Algorithm nameAlgorithm)
        {
            var payload = new
            {
                NameAlgorithm = nameAlgorithm.ToString()
            };
            string json = JsonSerializer.Serialize(payload);
            Url = $"http://{ip}:{port}/Train";
            Content = new(json, Encoding.UTF8, "application/json");
        }
    }
}
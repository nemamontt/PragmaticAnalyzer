using PragmaticAnalyzer.Abstractions;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace PragmaticAnalyzer.WorkingServer.Communication
{
    public class RequestCommunication : IRequest
    {
        public string Url { get; private set; }
        public StringContent? Content { get; private set; }

        public RequestCommunication(string userMessage, string port)
        {
            var payload = new
            {
                prompt = $"\n\n### User:\n{userMessage}\n\n### Assistant:\n",
                max_tokens = 512,
                temperature = 0.7,
                stop_sequence = new[] { "\n\n### User:", "\nUser:", "### User:" },
                rep_pen = 1.1,
                top_p = 0.95,
                n = 1
            };

            string json = JsonSerializer.Serialize(payload);
            Url = $"http://127.0.0.1:{port}/api/v1/generate";
            Content = new(json, Encoding.UTF8, "application/json");
        }
    }
}

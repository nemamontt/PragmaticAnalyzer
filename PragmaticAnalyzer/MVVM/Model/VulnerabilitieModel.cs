using Aspose.Cells;
using PragmaticAnalyzer.Configs;
using PragmaticAnalyzer.Databases;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;

namespace PragmaticAnalyzer.MVVM.Model
{
    public class VulnerabilitieModel
    {
        private readonly HttpClient _httpClient;
        private VulConfig _vulConfig;
        public event Action<string>? NotifyRequested;

        public VulnerabilitieModel(VulConfig vulConfig)
        {
            _vulConfig = vulConfig;
            var handler = new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                ServerCertificateCustomValidationCallback =
                (httpRequestMessage, cert, cetChain, policyErrors) => { return true; }
            };
            _httpClient = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(60)
            };
        }

        public async Task<ObservableCollection<Vulnerabilitie>> GetDatabase(CancellationToken ct)
        {
            int counter = 1;
            ObservableCollection<Vulnerabilitie> allVulnerabilities = [];

            ct.ThrowIfCancellationRequested();
            NotifyRequested?.Invoke("Начало работы с БД ФСТЭК");
            if (await GetByLink() is ObservableCollection<Vulnerabilitie> vulnerabilitiesLink)
            {
                foreach (var vulnerabilitie in vulnerabilitiesLink)
                {
                    allVulnerabilities.Add(vulnerabilitie);
                    counter++;
                }
            }
            NotifyRequested?.Invoke(" Завершение работы с БД ФСТЭК");

            ct.ThrowIfCancellationRequested();
            NotifyRequested?.Invoke("Начало работы с БД JVN");
      /*      if (await GetByPageParsing() is ObservableCollection<VulJvn> vulnerabilitiesPageParsing)
            {
                foreach (var vulnerabilitie in vulnerabilitiesPageParsing)
                {
                    vulnerabilitie.Id = $"GNA-{counter}";
                    vulnerabilitie.GuidId = Guid.NewGuid();
                    allVulnerabilities.Add(vulnerabilitie);
                    counter++;
                }
            }*/
            NotifyRequested?.Invoke(" Завершение работы с БД JVN");

            ct.ThrowIfCancellationRequested();
            NotifyRequested?.Invoke("Начало работы с БД NVD");
 /*           if (await GetByApiRequest() is ObservableCollection<VulNvd> vulnerabilitiesApiRequest)
            {
                foreach (var vulnerabilitie in vulnerabilitiesApiRequest)
                {
                    vulnerabilitie.Id = $"GNA-{counter}";
                    vulnerabilitie.GuidId = Guid.NewGuid();
                    allVulnerabilities.Add(vulnerabilitie);
                    counter++;
                }
            }*/
            NotifyRequested?.Invoke(" Завершение работы с БД NVD");

            return allVulnerabilities;
        }

        private async Task<ObservableCollection<Vulnerabilitie>> GetByLink()
        {
            ObservableCollection<Vulnerabilitie> vulnerabilities = [];
            using HttpRequestMessage request = new(HttpMethod.Get, _vulConfig.UrlFstec);
            NotifyRequested?.Invoke($" Обращение к {_vulConfig.UrlFstec}");
            using HttpResponseMessage response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                using Stream stream = await response.Content.ReadAsStreamAsync();
                using Workbook workbook = new(stream, new(LoadFormat.Xlsx));
                using Worksheet worksheet = workbook.Worksheets[0];
                var numberRows = worksheet.Cells.MaxDataRow;

                NotifyRequested?.Invoke($" Обработка полученной информации от {_vulConfig.UrlFstec}");
                // for (int rowIterator = 3; rowIterator < numberRows; rowIterator++)
                for (int rowIterator = 3; rowIterator < 3000; rowIterator++)
                {
                    Vulnerabilitie vulnerability = new()
                    {
                        Identifier = worksheet.Cells[rowIterator, 0].Value?.ToString() ?? "Информация отсутсвует",
                        Name = worksheet.Cells[rowIterator, 1].Value?.ToString() ?? "Информация отсутсвует",
                        Description = worksheet.Cells[rowIterator, 2].Value?.ToString() ?? "Информация отсутсвует",
                        Vendor = worksheet.Cells[rowIterator, 3].Value?.ToString() ?? "Информация отсутсвует",
                        NameSoftware = worksheet.Cells[rowIterator, 4].Value?.ToString() ?? "Информация отсутсвует",
                        Version = worksheet.Cells[rowIterator, 5].Value?.ToString() ?? "Информация отсутсвует",
                        Type = worksheet.Cells[rowIterator, 6].Value?.ToString() ?? "Информация отсутсвует",
                        NameOperatingSystem = worksheet.Cells[rowIterator, 7].Value?.ToString() ?? "Информация отсутсвует",
                        Class = worksheet.Cells[rowIterator, 8].Value?.ToString() ?? "Информация отсутсвует",
                        Date = worksheet.Cells[rowIterator, 9].Value?.ToString() ?? "Информация отсутсвует",
                        CvssTwo = worksheet.Cells[rowIterator, 10].Value?.ToString() ?? "Информация отсутсвует",
                        CvssThree = worksheet.Cells[rowIterator, 11].Value?.ToString() ?? "Информация отсутсвует",
                        DangerLevel = worksheet.Cells[rowIterator, 12].Value?.ToString() ?? "Информация отсутсвует",
                        Measure = worksheet.Cells[rowIterator, 13].Value?.ToString() ?? "Информация отсутсвует",
                        Exploit = worksheet.Cells[rowIterator, 14].Value?.ToString() ?? "Информация отсутсвует",
                        Information = worksheet.Cells[rowIterator, 15].Value?.ToString() ?? "Информация отсутсвует",
                        Links = worksheet.Cells[rowIterator, 16].Value?.ToString() ?? "Информация отсутсвует",
                        OtherIdentifier = worksheet.Cells[rowIterator, 17].Value?.ToString() ?? "Информация отсутсвует",
                        OtherInformation = worksheet.Cells[rowIterator, 18].Value?.ToString() ?? "Информация отсутсвует",
                        Incident = worksheet.Cells[rowIterator, 19].Value?.ToString() ?? "Информация отсутсвует",
                        OperatingMethod = worksheet.Cells[rowIterator, 20].Value?.ToString() ?? "Информация отсутсвует",
                        EliminationMethod = worksheet.Cells[rowIterator, 21].Value?.ToString() ?? "Информация отсутсвует",
                        DescriptionCwe = worksheet.Cells[rowIterator, 22].Value?.ToString() ?? "Информация отсутсвует",
                        Cwe = worksheet.Cells[rowIterator, 23].Value?.ToString() ?? "Информация отсутсвует"
                    };
                    vulnerabilities.Add(vulnerability);
                }
            }
            return vulnerabilities;
        }

        private async Task<ObservableCollection<Vulnerabilitie>?> GetByPageParsing()
        {
            throw new NotImplementedException();
        }

        private async Task<ObservableCollection<Vulnerabilitie>?> GetByApiRequest()
        {
            throw new NotImplementedException();
        }
    }
}
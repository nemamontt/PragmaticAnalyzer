using Aspose.Cells;
using PragmaticAnalyzer.Configs;
using PragmaticAnalyzer.Databases;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Xml.Serialization;

namespace PragmaticAnalyzer.MVVM.Model
{
    public class VulnerabilitieModel
    {
        private const int _resultsPerPageNvd = 2000;
        private readonly HttpClient _httpClient;
        private readonly VulConfig _vulConfig;
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

        public async Task<ObservableCollection<VulnerabilitieFstec>> GetByLink(CancellationToken ct)
        {
            ObservableCollection<VulnerabilitieFstec> vulnerabilities = [];
            using HttpRequestMessage request = new(HttpMethod.Get, _vulConfig.UrlFstec);
            NotifyRequested?.Invoke("Начало работы с БД ФСТЭК");
            NotifyRequested?.Invoke($"Обращение к {_vulConfig.UrlFstec}");
            using HttpResponseMessage response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                using Stream stream = await response.Content.ReadAsStreamAsync();
                using Workbook workbook = new(stream, new(LoadFormat.Xlsx));
                using Worksheet worksheet = workbook.Worksheets[0];
                var numberRows = worksheet.Cells.MaxDataRow;

                NotifyRequested?.Invoke($"Обработка полученной информации от {_vulConfig.UrlFstec}");
                // for (int rowIterator = 3; rowIterator < numberRows; rowIterator++)
                ct.ThrowIfCancellationRequested();
                for (int rowIterator = 3; rowIterator < 200; rowIterator++)
                {
                    VulnerabilitieFstec vulnerability = new()
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

        public async Task<ObservableCollection<VulnerabilitieNvd>?> GetByApiRequest(CancellationToken ct)
        {
            if (!string.IsNullOrWhiteSpace(_vulConfig.ApiKeyNvd))
            {
                _httpClient.DefaultRequestHeaders.Remove("apiKey");
                _httpClient.DefaultRequestHeaders.Add("apiKey", _vulConfig.ApiKeyNvd);
            }

            ObservableCollection<VulnerabilitieNvd> vulnerabilities = [];

            while (true)
            {
                var url = $"{_vulConfig.UrlNvd}?resultsPerPage={_resultsPerPageNvd}&startIndex={_vulConfig.StartIndexNvd}";

                try
                {
                    NotifyRequested?.Invoke($"Обращение к {url}");
                    var response = await _httpClient.GetAsync(url, ct);
                    response.EnsureSuccessStatusCode();

                    var json = await response.Content.ReadAsStringAsync(ct);

                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var apiResponse = JsonSerializer.Deserialize<NvdApiResponse>(json, options);

                    if (apiResponse?.Vulnerabilities == null || apiResponse.Vulnerabilities.Count == 0)
                    {
                        break;
                    }

                    foreach (var wrapper in apiResponse.Vulnerabilities)
                    {
                        var cve = wrapper.Cve;

                        var enDesc = cve.Descriptions
                            .FirstOrDefault(d => d.Lang.Equals("en", StringComparison.OrdinalIgnoreCase))?.Value
                            ?? cve.Descriptions.FirstOrDefault()?.Value
                            ?? string.Empty;

                        string vector = string.Empty;
                        if (!string.IsNullOrEmpty(cve.Metrics?.CvssMetricV31?.FirstOrDefault()?.CvssData?.VectorString))
                            vector = cve.Metrics.CvssMetricV31[0].CvssData.VectorString;
                        else if (!string.IsNullOrEmpty(cve.Metrics?.CvssMetricV30?.FirstOrDefault()?.CvssData?.VectorString))
                            vector = cve.Metrics.CvssMetricV30[0].CvssData.VectorString;
                        else if (!string.IsNullOrEmpty(cve.Metrics?.CvssMetricV2?.FirstOrDefault()?.CvssData?.VectorString))
                            vector = cve.Metrics.CvssMetricV2[0].CvssData.VectorString;

                        var references = new ObservableCollection<string>(
                            cve.References
                                .Where(r => !string.IsNullOrWhiteSpace(r.Url))
                                .Select(r => r.Url)
                        );

                        var vuln = new VulnerabilitieNvd
                        {
                            Identifier = cve.Id ?? "Информация отсутсвует",
                            Published = cve.Published.ToString("d") ?? "Информация отсутсвует",
                            LastModified = cve.LastModified.ToString("d") ?? "Информация отсутсвует",
                            VulnStatus = cve.VulnStatus ?? "Информация отсутсвует",
                            Description = enDesc ?? "Информация отсутсвует",
                            VectorString = vector ?? "Информация отсутсвует",
                            References = references
                        };

                        vulnerabilities.Add(vuln);
                        NotifyRequested?.Invoke($"Добавлкна запись {cve.Id}");
                    }

                    _vulConfig.StartIndexNvd += _resultsPerPageNvd;

                    var delayMs = string.IsNullOrWhiteSpace(_vulConfig.ApiKeyNvd) ? 7000 : 600;
                    await Task.Delay(delayMs, ct);
                }
                catch (Exception ex)
                {
                    NotifyRequested?.Invoke($"Ошибка при startIndex={_vulConfig.StartIndexNvd}: {ex.Message}");
                    break;
                }
            }
            return vulnerabilities;
        }

        public async Task<ObservableCollection<VulnerabilitieJvn>?> GetByPageParsing(CancellationToken ct)
        {
            ObservableCollection<VulnerabilitieJvn> vulnerabilities = [];
            var countYears = DateTime.Now.Year - _vulConfig.StartIndexJvn;
            for (int i = 0; i <= countYears; i++)
            {
                var items = await LoadYearAsync(_vulConfig.StartIndexJvn);
                var a = items.Count;
                if (items is null) continue;
                foreach (var item in items)
                {
                    vulnerabilities.Add(new VulnerabilitieJvn()
                    {
                        Identifier = item.Identifier ?? "Информация отсутсвует",
                        Description = item.Description ?? "Информация отсутсвует",
                        Link = item.Link ?? "Информация отсутсвует",
                        DateChange = item.Modified ?? "Информация отсутсвует",
                        DateAdded = item.Date ?? "Информация отсутсвует",
                        Name = item.Title ?? "Информация отсутсвует",
                        Cvss = item.Cvss?.Vector ?? "Информация отсутсвует"
                    });
                    var vul = vulnerabilities.Last();
                    foreach (var referenc in item.References)
                    {
                        vul.References.Add(referenc?.Url ?? "Информация отсутсвует");
                    }
                    foreach (var cpe in item.Cpes)
                    {
                        vul.Vendor.Add(cpe?.Vendor ?? "Информация отсутсвует");
                        vul.NameSoftware.Add(cpe?.Product ?? "Информация отсутсвует");
                        vul.VersionSoftware.Add(cpe?.Version ?? "Информация отсутсвует");
                    }
                    NotifyRequested?.Invoke($"Добавлена запись {item.Identifier}");
                }

                _vulConfig.StartIndexJvn++;
                await Task.Delay(1000, ct);
            }

            return vulnerabilities;

            async Task<List<Item>?> LoadYearAsync(int year)
            {
                string url = $"{_vulConfig.UrlJvn}{year}.rdf";
                NotifyRequested?.Invoke($"Обращение к {url}");
                try
                {
                    var xml = await _httpClient.GetStringAsync(url, ct);

                    xml = xml.Replace("  \"", "\"");

                    var serializer = new XmlSerializer(typeof(JvnRdf));
                    using var reader = new StringReader(xml);
                    var rdf = (JvnRdf)serializer.Deserialize(reader);

                    return rdf.Items;
                }
                catch (Exception ex)
                {
                    NotifyRequested?.Invoke($"Ошибка при обработке {url}.\nПодробности:\n{ex.Message}");
                    return null;
                }
            }
        }
    }
}
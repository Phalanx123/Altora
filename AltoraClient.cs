using Altora.Model;
using System.Text.Json;
using System.Text.Json.Serialization;
using Altora.Configuration;
using Microsoft.Extensions.Options;
using System.Text;
using Altora.Utilities;
using Microsoft.Extensions.Logging;

namespace Altora
{
    public class AltoraClient : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly ILogger<AltoraClient> _logger;
        private bool _disposed;

        public AltoraClient(IOptions<AltoraOptions> options, ILogger<AltoraClient> logger)
        {
            _logger = logger;
            _logger.LogInformation("Initializing AltoraClient for ClientId: {ClientId}", options.Value.ClientId);
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri($"https://api.userlogin.com.au/v1/{options.Value.ClientId}")
            };

            logger.LogInformation("BaseAddress set to: {BaseAddress}", _httpClient.BaseAddress);

            // Add default headers
            _httpClient.DefaultRequestHeaders.Add("X-Api-Key", options.Value.ApiKey);
            _httpClient.DefaultRequestHeaders.Add("X-Api-Secret", options.Value.ApiSecret);
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            LogCredentials(options.Value);
            // Configure JSON serializer options
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
        }

        private void LogCredentials(AltoraOptions options)
        {
            _logger.LogInformation(
                "Default headers added: X-Api-Key=***{ApiKey}, X-Api-Secret=***{ApiSecret}, ClientId=***{ClientId}",
                SafeTailUtil.SafeTail(options.ApiKey),
                SafeTailUtil.SafeTail(options.ApiSecret),
                SafeTailUtil.SafeTail(options.ClientId));
        }

        // Alternative constructor for DI with HttpClientFactory
        public AltoraClient(HttpClient httpClient, IOptions<AltoraOptions> options, ILogger<AltoraClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _httpClient.BaseAddress = new Uri($"https://api.userlogin.com.au/v1/{options.Value.ClientId}");

            // Add default headers
            _httpClient.DefaultRequestHeaders.Add("X-Api-Key", options.Value.ApiKey);
            _httpClient.DefaultRequestHeaders.Add("X-Api-Secret", options.Value.ApiSecret);
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            LogCredentials(options.Value);
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
        }

        /// <summary>
        /// Gets Altora Worker
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<AltoraWorker?> GetWorkerAsync(int workerId, bool includeCustomFields)
        {
            var url = $"users/{workerId}";
            if (includeCustomFields)
                url += "?customfields=1";

            var response = await _httpClient.GetAsync(url);
            await EnsureSuccessStatusCode(response);

            var json = await response.Content.ReadAsStringAsync();
            var worker = JsonSerializer.Deserialize<AltoraWorker>(json, _jsonOptions);

            if (worker != null)
            {
                var companies = await GetCompaniesAsync();
                worker.AltoraCompany = companies.SingleOrDefault(x => x.Id == worker.CompanyId);
            }

            return worker;
        }

        /// <summary>
        /// Gets Altora Courses
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<List<AltoraCourse>> GetCoursesAsync()
        {
            var response = await _httpClient.GetAsync("courses");
            await EnsureSuccessStatusCode(response);

            var json = await response.Content.ReadAsStringAsync();
            var courses = JsonSerializer.Deserialize<IEnumerable<AltoraCourse>>(json, _jsonOptions);
            return courses?.ToList() ?? new List<AltoraCourse>();
        }

        /// <summary>
        /// Gets Altora Documents
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<IEnumerable<AltoraDocument>> GetDocumentsAsync()
        {
            var response = await _httpClient.GetAsync("documents");
            await EnsureSuccessStatusCode(response);

            var json = await response.Content.ReadAsStringAsync();
            var documents = JsonSerializer.Deserialize<IEnumerable<AltoraDocument>>(json, _jsonOptions);
            return documents ?? [];
        }

        /// <summary>
        /// Gets Altora Forms
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<IEnumerable<AltoraForm>> GetFormsAsync()
        {
            var response = await _httpClient.GetAsync("forms");
            await EnsureSuccessStatusCode(response);

            var json = await response.Content.ReadAsStringAsync();
            var forms = JsonSerializer.Deserialize<IEnumerable<AltoraForm>>(json, _jsonOptions);
            return forms ?? [];
        }

        /// <summary>
        /// Gets Altora Acknowledgements
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<IEnumerable<AltoraAcknowledgement>> GetAcknowledgementsAsync()
        {
            var response = await _httpClient.GetAsync("acknowledgements");
            await EnsureSuccessStatusCode(response);

            var json = await response.Content.ReadAsStringAsync();
            var acknowledgements = JsonSerializer.Deserialize<IEnumerable<AltoraAcknowledgement>>(json, _jsonOptions);
            return acknowledgements ?? [];
        }

        /// <summary>
        /// Gets Altora Programs
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<IEnumerable<AltoraProgram>> GetProgramsAsync()
        {
            var response = await _httpClient.GetAsync("programs");
            await EnsureSuccessStatusCode(response);

            var json = await response.Content.ReadAsStringAsync();
            var programs = JsonSerializer.Deserialize<IEnumerable<AltoraProgram>>(json, _jsonOptions);
            return programs ?? [];
        }

        /// <summary>
        /// Gets Altora Workers
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<IEnumerable<AltoraWorker>> GetWorkersAsync(AltoraWorkerSearchParameters? parameters)
        {
            var url = "users";
            var queryParams = new List<string>();

            if (!string.IsNullOrWhiteSpace(parameters?.FirstName))
                queryParams.Add($"FirstName={Uri.EscapeDataString(parameters.FirstName)}");
            if (!string.IsNullOrWhiteSpace(parameters?.LastName))
                queryParams.Add($"LastName={Uri.EscapeDataString(parameters.LastName)}");
            if (!string.IsNullOrWhiteSpace(parameters?.Email))
                queryParams.Add($"Email={Uri.EscapeDataString(parameters.Email)}");

            if (queryParams.Any())
                url += "?" + string.Join("&", queryParams);

            var response = await _httpClient.GetAsync(url);
            await EnsureSuccessStatusCode(response);

            var json = await response.Content.ReadAsStringAsync();
            var workers = JsonSerializer.Deserialize<List<AltoraWorker>>(json, _jsonOptions)
                          ?? [];
            if (workers.Count == 0)
                return [];
            var companies = (await GetCompaniesAsync()).ToList();
            foreach (var worker in workers)
                worker.AltoraCompany = companies.SingleOrDefault(x => x.Id == worker.CompanyId);

            return workers;
        }

        /// <summary>
        /// Gets Altora Workers from specific company
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<IEnumerable<AltoraWorker>> GetWorkersAsync(string companyName)
        {
            var workers = await GetWorkersAsync(new AltoraWorkerSearchParameters());
            return workers.Where(x => x.AltoraCompany!.Name == companyName);
        }

        /// <summary>
        /// Gets Altora Workers from specific company
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<IEnumerable<AltoraWorker>> GetWorkersAsync(int companyId)
        {
            var workers = await GetWorkersAsync(new AltoraWorkerSearchParameters());
            return workers.Where(x => x.AltoraCompany!.Id == companyId);
        }

        /// <summary>
        /// Gets Altora Companies
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<IEnumerable<AltoraCompany>> GetCompaniesAsync()
        {
            var response = await _httpClient.GetAsync("companies");
            await EnsureSuccessStatusCode(response);

            var json = await response.Content.ReadAsStringAsync();
            var companies = JsonSerializer.Deserialize<IEnumerable<AltoraCompany>>(json, _jsonOptions);
            return companies ?? [];
        }

        /// <summary>
        /// Gets Altora Company Admins
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<int[]> GetCompanyAdminsAsync(string companyId)
        {
            var response = await _httpClient.GetAsync($"companies/{companyId}/admins");
            await EnsureSuccessStatusCode(response);

            var json = await response.Content.ReadAsStringAsync();
            var admins = JsonSerializer.Deserialize<int[]>(json, _jsonOptions);
            return admins ?? [];
        }

        /// <summary>
        /// Set Altora Company Status
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<AltoraRequestResponse> SetCompanyStatusAsync(string companyId, bool active)
        {
            var content = new StringContent((active ? 1 : 0).ToString(), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"companies/{companyId}/active", content);
            await EnsureSuccessStatusCode(response);

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<AltoraRequestResponse>(json, _jsonOptions);
            return result ?? throw new Exception("Failed to deserialize response");
        }

        /// <summary>
        /// Gets Worker Document
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<AltoraWorkerDocument?> GetWorkerDocumentAsync(int workerId, int documentId)
        {
            var url = $"users/{workerId}/documents?documentid={documentId}";
            var response = await _httpClient.GetAsync(url);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;

            await EnsureSuccessStatusCode(response);

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<AltoraWorkerDocument>(json, _jsonOptions);
        }

        /// <summary>
        /// Gets Worker Acknowledgement
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<AltoraWorkerAcknowledgement?> GetWorkerAcknowledgementAsync(int workerId,
            int acknowledgementId)
        {
            try
            {
                var url = $"users/{workerId}/acknowledgements?acknowledgementid={acknowledgementId}";
                var response = await _httpClient.GetAsync(url);

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return null;

                await EnsureSuccessStatusCode(response);

                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<AltoraWorkerAcknowledgement>(json, _jsonOptions);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        /// <summary>
        /// Gets Worker Form Items
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<AltoraWorkerForm?> GetWorkerFormAsync(int workerId, int formId)
        {
            try
            {
                var url = $"users/{workerId}/forms?formid={formId}";
                var response = await _httpClient.GetAsync(url);

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return null;

                await EnsureSuccessStatusCode(response);

                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<AltoraWorkerForm>(json, _jsonOptions);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        /// <summary>
        /// Gets courses previously completed by a user
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<IEnumerable<AltoraWorkerCourseCompleted>> GetWorkerCoursesAsync(int workerId, int? courseId)
        {
            var url = $"users/{workerId}/courses";
            if (courseId.HasValue)
                url += $"?courseid={courseId.Value}";

            var response = await _httpClient.GetAsync(url);
            await EnsureSuccessStatusCode(response);

            var json = await response.Content.ReadAsStringAsync();
            var courses = JsonSerializer.Deserialize<List<AltoraWorkerCourseCompleted>>(json, _jsonOptions);
            return courses ?? new List<AltoraWorkerCourseCompleted>();
        }

        /// <summary>
        /// Gets programs assigned to specified user
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<int[]> GetWorkerProgramsAsync(int workerId)
        {
            var response = await _httpClient.GetAsync($"users/{workerId}/programs");
            await EnsureSuccessStatusCode(response);

            var json = await response.Content.ReadAsStringAsync();
            var programs = JsonSerializer.Deserialize<int[]>(json, _jsonOptions);
            return programs ?? [];
        }

        /// <summary>
        /// Sets programs assigned to specified user
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<AltoraRequestResponse> SetWorkerProgramsAsync(int workerId, int[] programs)
        {
            var requestBody = new { Programs = programs };
            var json = JsonSerializer.Serialize(requestBody, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"users/{workerId}/programs", content);
            await EnsureSuccessStatusCode(response);

            var responseJson = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<AltoraRequestResponse>(responseJson, _jsonOptions);
            return result ?? throw new Exception("Failed to deserialize response");
        }

        /// <summary>
        /// Sets a users status to active or inactive
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<AltoraRequestResponse> SetWorkerStatusAsync(int workerId, bool active)
        {
            var content = new StringContent((active ? 1 : 0).ToString(), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"users/{workerId}/active", content);
            await EnsureSuccessStatusCode(response);

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<AltoraRequestResponse>(json, _jsonOptions);
            return result ?? throw new Exception("Failed to deserialize response");
        }

        /// <summary>
        /// Add Worker
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<AltoraAddWorkerResponse> AddWorkerAsync(AltoraWorker worker)
        {
            var jsonOptions = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var json = JsonSerializer.Serialize(worker, jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("users/", content);
            await EnsureSuccessStatusCode(response);

            var responseJson = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<AltoraAddWorkerResponse>(responseJson, _jsonOptions);
            return result ?? throw new Exception("Failed to deserialize response");
        }

        public async Task<AltoraRequestResponse> UpdateWorker(AltoraWorker worker)
        {
            var jsonOptions = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var json = JsonSerializer.Serialize(worker, jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PatchAsync($"users/{worker.Id}", content);
            await EnsureSuccessStatusCode(response);

            var responseJson = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<AltoraRequestResponse>(responseJson, _jsonOptions);
            return result ?? throw new Exception("Failed to deserialize response");
        }

        private async Task EnsureSuccessStatusCode(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Request failed with status {response.StatusCode}: {errorContent}");
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed || !disposing)
            {
                return;
            }

            _httpClient.Dispose();
            _disposed = true;
        }
    }
}
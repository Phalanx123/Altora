using Altora.Model;
using System.Text.Json;
using System.Text.Json.Serialization;
using Altora.Configuration;
using Microsoft.Extensions.Options;
using System.Text;
using Altora.Model.Requests;
using Altora.Model.Responses;
using Microsoft.Extensions.Logging;

namespace Altora
{
    public class AltoraClient : IDisposable
    {
        /// <summary>The Altora API root; the tenant's client id is the next path segment.</summary>
        public const string ApiRoot = "https://api.userlogin.com.au/v1/";

        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNameCaseInsensitive = true
        };
        private readonly ILogger<AltoraClient> _logger;
        private readonly bool _ownsHttpClient;
        private bool _disposed;

        /// <summary>
        /// Builds a client over an HttpClient of its own. Every instance opens its own connections, so prefer
        /// <see cref="AltoraClientServiceCollectionExtensions.AddAltoraClient"/> and inject the client.
        /// </summary>
        [Obsolete("Creates an HttpClient per instance. Register the client with services.AddAltoraClient() and inject AltoraClient instead.")]
        public AltoraClient(IOptions<AltoraOptions> options, ILogger<AltoraClient> logger)
            : this(new HttpClient(), options, logger)
        {
            _ownsHttpClient = true;
        }

        /// <summary>
        /// Typed client constructor used by IHttpClientFactory. A client already configured by
        /// <see cref="AltoraClientServiceCollectionExtensions.AddAltoraClient"/> is used as it is.
        /// </summary>
        public AltoraClient(HttpClient httpClient, IOptions<AltoraOptions> options, ILogger<AltoraClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;

            // The factory hands every typed client a fresh HttpClient (the handler underneath is pooled), so
            // this runs once per injection; it stays quiet because a scoped service resolves one per circuit.
            if (_httpClient.BaseAddress is null)
                Configure(_httpClient, options.Value);

            _logger.LogDebug("Altora client created for {BaseAddress}", _httpClient.BaseAddress);
        }

        /// <summary>
        /// Points an HttpClient at the tenant's API and adds the credential headers. Safe to call twice: each
        /// header is replaced, never added a second time.
        /// </summary>
        public static void Configure(HttpClient httpClient, AltoraOptions options)
        {
            httpClient.BaseAddress = BaseAddressFor(options.ClientId);
            SetHeader(httpClient, "X-Api-Key", options.ApiKey);
            SetHeader(httpClient, "X-Api-Secret", options.ApiSecret);
            SetHeader(httpClient, "Accept", "application/json");
        }

        /// <summary>The API base address for a tenant's client id.</summary>
        public static Uri BaseAddressFor(string clientId) => new($"{ApiRoot}{clientId}/");

        private static void SetHeader(HttpClient httpClient, string name, string? value)
        {
            httpClient.DefaultRequestHeaders.Remove(name);
            if (!string.IsNullOrWhiteSpace(value))
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation(name, value);
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
        public async Task<CreateWorkerResponse> AddWorkerAsync(CreateWorkerRequest worker)
        {
            var jsonOptions = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            };

            var json = JsonSerializer.Serialize(worker, jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("users/", content);
            await EnsureSuccessStatusCode(response);

            var responseJson = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<CreateWorkerResponse>(responseJson, _jsonOptions);
            return result ?? throw new Exception("Failed to deserialize response");
        }

        /// <summary>
        /// Updates a user's details (PATCH users/{id}). Only the fields set on <paramref name="worker"/>
        /// are sent, so anything left null keeps the value Altora already holds.
        /// </summary>
        public async Task<AltoraRequestResponse> UpdateWorkerAsync(UpdateWorkerRequest worker,
            CancellationToken cancellationToken = default)
        {
            var json = JsonSerializer.Serialize(worker, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PatchAsync($"users/{worker.Id}", content, cancellationToken);
            await EnsureSuccessStatusCode(response);

            var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonSerializer.Deserialize<AltoraRequestResponse>(responseJson, _jsonOptions);
            return result ?? throw new Exception("Failed to deserialize response");
        }

        /// <summary>
        /// Kept for callers built against 1.0.20; the same PATCH as <see cref="UpdateWorkerAsync"/>.
        /// </summary>
        public Task<AltoraRequestResponse> UpdateWorker(UpdateWorkerRequest worker) => UpdateWorkerAsync(worker);

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

            if (_ownsHttpClient)
                _httpClient.Dispose();
            _disposed = true;
        }
    }
}
using Altora.Model;
using RestSharp;
using RestSharp.Serializers.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Altora
{
    public class AltoraClient
    {
        private readonly string ApiKey;
        private readonly string ApiSecret;
        private readonly string ClientID;
        public RestClient Client { get; set; }


        public AltoraClient(string apiKey, string apiSecret, string clientID)
        {
            ApiKey = apiKey;
            ApiSecret = apiSecret;
            ClientID = clientID;
            Client = new RestClient(@$"https://api.userlogin.com.au/v1/{ClientID}/");
            Client.AddDefaultHeaders(new Dictionary<string, string> { { "x-api-key", ApiKey }, { "x-api-secret", ApiSecret } });
                  }

        /// <summary>
        /// Gets Altora Worker
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>

        public async Task<AltoraWorker> GetWorkerAsync(int workerID)
        {
            var request = new RestRequest($"/users/{workerID}", Method.Get);
            var result = await Client.ExecuteAsync<AltoraWorker>(request);
            if (result.IsSuccessful && result.Data != null)
            {
                var companies = await GetCompaniesAsync();
                result.Data.AltoraCompany = companies.SingleOrDefault(x => x.Id == result.Data.CompanyId);
                return result.Data;
            }
            throw new Exception(result.ErrorMessage);

        }

        /// <summary>
        /// Gets Altora Courses
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>

        public async Task<List<AltoraCourse>> GetCoursesAsync()
        {
            var request = new RestRequest("/courses", Method.Get);
            var result = await Client.ExecuteAsync<IEnumerable<AltoraCourse>>(request);
            if (result.IsSuccessful && result.Data != null)
                return result.Data.ToList();
            throw new Exception(result.ErrorMessage);

        }

        /// <summary>
        /// Gets Altora Documents
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>

        public async Task<IEnumerable<AltoraDocument>> GetDocumentsAsync()
        {
            var request = new RestRequest("/documents", Method.Get);
            var result = await Client.ExecuteAsync<IEnumerable<AltoraDocument>>(request);
            if (result.IsSuccessful && result.Data != null)
                return result.Data;
            throw new Exception(result.ErrorMessage);

        }

        /// <summary>
        /// Gets Altora Forms
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>

        public async Task<IEnumerable<AltoraForm>> GetFormsAsync()
        {
            var request = new RestRequest("/forms", Method.Get);
            var result = await Client.ExecuteAsync<IEnumerable<AltoraForm>>(request);
            if (result.IsSuccessful && result.Data != null)
                return result.Data;
            throw new Exception(result.ErrorMessage);

        }

        /// <summary>
        /// Gets Altora Acknowledgements
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>

        public async Task<IEnumerable<AltoraAcknowledgement>> GetAcknowledgementsAsync()
        {
            var request = new RestRequest("/acknowledgements", Method.Get);
            var result = await Client.ExecuteAsync<IEnumerable<AltoraAcknowledgement>>(request);
            if (result.IsSuccessful && result.Data != null)
                return result.Data;
            throw new Exception(result.ErrorMessage);

        }

        /// <summary>
        /// Gets Altora Programs
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>

        public async Task<IEnumerable<AltoraProgram>> GetProgramsAsync()
        {
            var request = new RestRequest("/programs", Method.Get);
            var result = await Client.ExecuteAsync<IEnumerable<AltoraProgram>>(request);
            if (result.IsSuccessful && result.Data != null)
                return result.Data;
            throw new Exception(result.ErrorMessage);

        }

        /// <summary>
        /// Gets Altora Workers
        /// </summary>
        /// <param name="firstname">Employees first name to filter by</param>
        /// <param name="lastname">Employees last name to filter by</param>
        /// <param name="email">Employees email to filter by</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<IEnumerable<AltoraWorker>> GetWorkersAsync(string? firstname = null, string? lastname = null, string? email = null)
        {
            var request = new RestRequest("/users", Method.Get);
            if (firstname != null)
                request.AddQueryParameter(nameof(firstname), firstname);
            if (lastname != null)
                request.AddQueryParameter(nameof(lastname), lastname);
            if (email != null)
                request.AddQueryParameter(nameof(email), email);

            var result = await Client.ExecuteAsync<IEnumerable<AltoraWorker>>(request);
            if (result.IsSuccessful && result.Data != null)
            {
                var companies = await GetCompaniesAsync();
                foreach (var worker in result.Data)
                    worker.AltoraCompany = companies.SingleOrDefault(x => x.Id == worker.CompanyId);
                return result.Data;
            }
            throw new Exception(result.ErrorMessage);

        }


        /// <summary>
        /// Gets Altora Workers from specific company
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>

        public async Task<IEnumerable<AltoraWorker>> GetWorkersAsync(string companyName)
        {
            var workers = await GetWorkersAsync();
            return workers.Where(x => x.AltoraCompany!.Name == companyName);
        }
        /// <summary>
        /// Gets Altora Workers from specific company
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>

        public async Task<IEnumerable<AltoraWorker>> GetWorkersAsync(int companyID)
        {
            var workers = await GetWorkersAsync();
            return workers.Where(x => x.AltoraCompany!.Id == companyID);
        }
        /// <summary>
        /// Gets Altora Companies
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<IEnumerable<AltoraCompany>> GetCompaniesAsync()
        {
            var request = new RestRequest("/companies", Method.Get);
            var result = await Client.ExecuteAsync<IEnumerable<AltoraCompany>>(request);
            if (result.IsSuccessful && result.Data != null)
                return result.Data;
            throw new Exception(result.ErrorMessage);
        }
        /// <summary>
        /// Gets Altora Company Admins
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<int[]> GetCompanyAdminsAsync(string companyId)
        {
            var request = new RestRequest($"/companies/{companyId}/admins", Method.Get);
            var result = await Client.ExecuteAsync<int[]>(request);
            if (result.IsSuccessful && result.Data != null)
                return result.Data;
            throw new Exception(result.ErrorMessage);
        }

        /// <summary>
        /// Set Altora Company Status
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<AltoraRequestResponse> SetCompanyStatusAsync(string companyId, bool active)
        {
            var request = new RestRequest($"/companies/{companyId}/active", Method.Put);
            request.AddBody(active ? 1 : 0);
            var result = await Client.ExecuteAsync<AltoraRequestResponse>(request);
            if (result.IsSuccessful && result.Data != null)
                return result.Data;
            throw new Exception(result.ErrorMessage);
        }

        /// <summary>
        /// Gets Worker Document
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>

        public async Task<AltoraWorkerDocument?> GetWorkerDocumentAsync(int workerId, int documentId)
        {
                var request = new RestRequest($"/users/{workerId}/documents", Method.Get);
                request.AddQueryParameter("documentid", documentId);
                var result = await Client.ExecuteAsync<AltoraWorkerDocument>(request);
                return result switch
                {
                    { IsSuccessful: true, Data: not null } => result.Data,
                    { IsSuccessful: true, Data: null } => null,
                    _ => throw new Exception(result.ErrorMessage)
                };
            }

        /// <summary>
        /// Gets Worker Documents
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>

        public async Task<AltoraWorkerAcknowledgement> GetWorkerAcknowledgementAsync(int workerId, int acknowledgementId)
        {
            var request = new RestRequest($"/users/{workerId}/acknowledgements", Method.Get);
            request.AddQueryParameter("acknowledgementid", acknowledgementId);
            var result = await Client.ExecuteAsync<AltoraWorkerAcknowledgement>(request);
            if (result.IsSuccessful && result.Data != null)
                return result.Data;
            throw new Exception(result.ErrorMessage);

        }
        /// <summary>
        /// Gets Worker Form Items
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>

        public async Task<AltoraWorkerForm> GetWorkerFormAsync(int workerId, int formId)
        {
            var request = new RestRequest($"/users/{workerId}/forms", Method.Get);
            request.AddQueryParameter("formid", formId);
            var result = await Client.ExecuteAsync<AltoraWorkerForm>(request);
            if (result.IsSuccessful && result.Data != null)
                return result.Data;
            throw new Exception(result.ErrorMessage);

        }

        /// <summary>
        /// Gets courses previously completed by a user
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>

        public async Task<IEnumerable<AltoraWorkerCourseCompleted>> GetWorkerCoursesAsync(int workerId, int? courseId)
        {
            var request = new RestRequest($"/users/{workerId}/courses", Method.Get);
            if (courseId != null)
                request.AddQueryParameter("courseid", courseId.Value);
            var result = await Client.ExecuteAsync<IEnumerable<AltoraWorkerCourseCompleted>>(request);
            if (result.IsSuccessful && result.Data != null)
                return result.Data;
            throw new Exception(result.ErrorMessage);

        }
        /// <summary>
        /// Gets programs assigned to specified user
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>

        public async Task<int[]> GetWorkerProgramsAsync(int workerId)
        {
            var request = new RestRequest($"/users/{workerId}/programs", Method.Get);
            var result = await Client.ExecuteAsync<int[]>(request);
            if (result.IsSuccessful && result.Data != null)
                return result.Data;
            throw new Exception(result.ErrorMessage);

        }

        /// <summary>
        /// Sets programs assigned to specified user
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>

        public async Task<AltoraRequestResponse> SetWorkerProgramsAsync(int workerId, int[] programs)
        {
            var request = new RestRequest($"/users/{workerId}/programs", Method.Put);
            request.AddJsonBody(new
            {
                Programs = programs
            });
            var result = await Client.ExecuteAsync<AltoraRequestResponse>(request);
            if (result.IsSuccessful && result.Data != null)
                return result.Data;
            throw new Exception(result.ErrorMessage);
        }

        /// <summary>
        /// Sets a users status to active or inactive
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>

        public async Task<AltoraRequestResponse> SetWorkerStatusAsync(int workerId, bool active)
        {
            var request = new RestRequest($"/users/{workerId}/active", Method.Put);
            request.AddBody(active ? 1 : 0);
            var result = await Client.ExecuteAsync<AltoraRequestResponse>(request);
            if (result.IsSuccessful && result.Data != null)
                return result.Data;
            throw new Exception(result.ErrorMessage);
        }

        /// <summary>
        /// Add Worker
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>

        public async Task<AltoraAddWorkerResponse> AddWorkerAsync(AltoraWorker worker)
        {
            var request = new RestRequest($"/users/", Method.Post);
            JsonSerializerOptions options = new()
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            };

            var json = JsonSerializer.Serialize(worker, options);
            request.AddJsonBody(json);
            var result = await Client.ExecuteAsync<AltoraAddWorkerResponse>(request);
            if (result.IsSuccessful && result.Data != null)
                return result.Data;
            throw new Exception(result.ErrorMessage);

        }
    }
}

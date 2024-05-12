using Altora.Model;
using RestSharp;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Altora
{
    public class AltoraClient
    {
        private RestClient Client { get; set; }
        
        public AltoraClient(string apiKey, string apiSecret, string clientID)
        {
            Client = new RestClient(@$"https://api.userlogin.com.au/v1/{clientID}/");
            Client.AddDefaultHeaders(new Dictionary<string, string>
                { { "x-api-key", apiKey }, { "x-api-secret", apiSecret } });
        }

        /// <summary>
        /// Gets Altora Worker
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<AltoraWorker?> GetWorkerAsync(int workerID, bool includeCustomFields)
        {
            var request = new RestRequest($"/users/{workerID}");
            if(includeCustomFields)
                request.AddQueryParameter("customfields", "1");
            var result = await Client.ExecuteAsync<AltoraWorker>(request);
            
            if (result is not { IsSuccessful: true, Data: not null }) throw new Exception(result.ErrorMessage);
            
            var companies = await GetCompaniesAsync();
            result.Data.AltoraCompany = companies.SingleOrDefault(x => x.Id == result.Data.CompanyId);
            return result.Data;

        }

        /// <summary>
        /// Gets Altora Courses
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<List<AltoraCourse>> GetCoursesAsync()
        {
            var request = new RestRequest("/courses");
            var result = await Client.ExecuteAsync<IEnumerable<AltoraCourse>>(request);
            if (result is { IsSuccessful: true, Data: not null })
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
            var request = new RestRequest("/documents");
            var result = await Client.ExecuteAsync<IEnumerable<AltoraDocument>>(request);
            if (result is { IsSuccessful: true, Data: not null })
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
            var request = new RestRequest("/forms");
            var result = await Client.ExecuteAsync<IEnumerable<AltoraForm>>(request);
            if (result is { IsSuccessful: true, Data: not null })
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
            var request = new RestRequest("/acknowledgements");
            var result = await Client.ExecuteAsync<IEnumerable<AltoraAcknowledgement>>(request);
            if (result is { IsSuccessful: true, Data: not null })
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
            var request = new RestRequest("/programs");
            var result = await Client.ExecuteAsync<IEnumerable<AltoraProgram>>(request);
            if (result is { IsSuccessful: true, Data: not null })
                return result.Data;
            throw new Exception(result.ErrorMessage);
        }

        /// <summary>
        /// Gets Altora Workers
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<IEnumerable<AltoraWorker>> GetWorkersAsync(AltoraWorkerSearchParameters? parameters)
        {
            var request = new RestRequest("/users");
            if (!string.IsNullOrWhiteSpace(parameters?.FirstName))
                request.AddQueryParameter(nameof(parameters.FirstName), parameters.FirstName);
            if (!string.IsNullOrWhiteSpace(parameters?.LastName))
                request.AddQueryParameter(nameof(parameters.LastName), parameters.LastName);
            if (!string.IsNullOrWhiteSpace(parameters?.Email))
                request.AddQueryParameter(nameof(parameters.Email), parameters.Email);

            var result = await Client.ExecuteAsync<IEnumerable<AltoraWorker>>(request);
            if (!result.IsSuccessful || result.Data == null) throw new Exception(result.ErrorMessage);
            var companies = (await GetCompaniesAsync()).ToList();
            foreach (var worker in result.Data)
                worker.AltoraCompany = companies.SingleOrDefault(x => x.Id == worker.CompanyId);
            return result.Data;

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
        public async Task<IEnumerable<AltoraWorker>> GetWorkersAsync(int companyID)
        {
            var workers = await GetWorkersAsync(new AltoraWorkerSearchParameters());
            return workers.Where(x => x.AltoraCompany!.Id == companyID);
        }

        /// <summary>
        /// Gets Altora Companies
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<IEnumerable<AltoraCompany>> GetCompaniesAsync()
        {
            var request = new RestRequest("/companies");
            var result = await Client.ExecuteAsync<IEnumerable<AltoraCompany>>(request);
            if (result is { IsSuccessful: true, Data: not null })
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
            var request = new RestRequest($"/companies/{companyId}/admins");
            var result = await Client.ExecuteAsync<int[]>(request);
            if (result is { IsSuccessful: true, Data: not null })
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
            if (result is { IsSuccessful: true, Data: not null })
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
            var request = new RestRequest($"/users/{workerId}/documents");
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
        public async Task<AltoraWorkerAcknowledgement?> GetWorkerAcknowledgementAsync(int workerId,
            int acknowledgementId)
        {
            try
            {
                var request = new RestRequest($"/users/{workerId}/acknowledgements");
                request.AddQueryParameter("acknowledgementid", acknowledgementId);
                var result = await Client.ExecuteAsync<AltoraWorkerAcknowledgement>(request);
                return result is { IsSuccessful: true, Data: not null } ? result.Data : null;
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
                var request = new RestRequest($"/users/{workerId}/forms");
                request.AddQueryParameter("formid", formId);
                var result = await Client.ExecuteAsync<AltoraWorkerForm>(request);
                return result is { IsSuccessful: true, Data: not null } ? result.Data : null;
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
            var request = new RestRequest($"/users/{workerId}/courses");
            if (courseId != null)
                request.AddQueryParameter("courseid", courseId.Value);
            var result = await Client.ExecuteAsync<List<AltoraWorkerCourseCompleted>>(request);
            return result switch
            {
                { IsSuccessful: true, Data: not null } => result.Data,
                { IsSuccessful: true, Data: null } => [],
                _ => throw new Exception(result.ErrorMessage)
            };
        }

        /// <summary>
        /// Gets programs assigned to specified user
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<int[]> GetWorkerProgramsAsync(int workerId)
        {
            var request = new RestRequest($"/users/{workerId}/programs");
            var result = await Client.ExecuteAsync<int[]>(request);
            if (result is { IsSuccessful: true, Data: not null })
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
            if (result is { IsSuccessful: true, Data: not null })
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
            if (result is { IsSuccessful: true, Data: not null })
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
            if (result is { IsSuccessful: true, Data: not null })
                return result.Data;
            throw new Exception(result.ErrorMessage);
        }
    }
}
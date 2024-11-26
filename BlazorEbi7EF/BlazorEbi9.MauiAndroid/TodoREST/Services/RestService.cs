using BlazorEbi7.Model.Entidades;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.Json;

using TodoREST.Models;

namespace TodoREST.Services
{
    public class RestService : IRestService
    {
        HttpClient _client;
        JsonSerializerOptions _serializerOptions;
        IHttpsClientHandlerService _httpsClientHandlerService;

        public List<UsuarioSet> Items { get; private set; }
        private readonly IHttpClientFactory _httpClientFactory;


        public RestService(IHttpsClientHandlerService service, IHttpClientFactory httpClientFactory)
        {
#if DEBUG
            _httpClientFactory = httpClientFactory;
            _httpsClientHandlerService = service;
            HttpMessageHandler handler = _httpsClientHandlerService.GetPlatformMessageHandler();
            
            if (handler != null)
                _client = new HttpClient(handler);
            else
                _client =  _httpClientFactory.CreateClient();
#else
            _client = new HttpClient();
#endif
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
        }

        public async Task<List<UsuarioSet>> RefreshDataAsync()
        {
            Items = new List<UsuarioSet>();

            Uri uri = new Uri(string.Format(Constants.RestUrl, string.Empty));
            try
            {
                HttpResponseMessage response = await _client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    Items = JsonSerializer.Deserialize<List<UsuarioSet>>(content, _serializerOptions);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }

            return Items;
        }

        public async Task SaveTodoItemAsync(UsuarioSet item, bool isNewItem = false)
        {
            Uri uri = new Uri(string.Format(Constants.RestUrl, string.Empty));

            try
            {
                string json = JsonSerializer.Serialize<UsuarioSet>(item, _serializerOptions);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = null;
                if (isNewItem)
                    response = await _client.PostAsync(uri, content);
                else
                    response = await _client.PutAsync(uri, content);

                if (response.IsSuccessStatusCode)
                    Debug.WriteLine(@"\tTodoItem successfully saved.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }
        }

        public async Task DeleteTodoItemAsync(string id)
        {
            Uri uri = new Uri(string.Format(Constants.RestUrl, id));

            try
            {
                HttpResponseMessage response = await _client.DeleteAsync(uri);
                if (response.IsSuccessStatusCode)
                    Debug.WriteLine(@"\tTodoItem successfully deleted.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }
        }
    }
}

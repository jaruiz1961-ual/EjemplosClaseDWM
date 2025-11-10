using BlazorEbi9.Model;
using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text.Json;

using System.Text;
using System.Data.SqlTypes;
using BlazorEbi9.Data.Services;
using System.Net.NetworkInformation;

using BlazorEbi9.Data;
using BlazorEbi9.Model.Entidades;
using BlazorEbi9.Model.Interfaces;
using System.Linq.Expressions;
using System.Linq;
using Microsoft.VisualBasic;

namespace BlazorEbi9.RestfullCore.Repositories
{
    public interface IHttpsClientHandlerService
    {
        HttpMessageHandler GetPlatformMessageHandler();
    }
    public class RepositorioBaseAsyncR<T> : IRepositoryBaseAsync<T> where T : class
    {
        //https://restclient.dalsoft.io/

        private readonly HttpClient httpClient;
        private string _baseAddress;
        IHttpsClientHandlerService _httpsClientHandlerService;
        string _url;

        public RepositorioBaseAsyncR(HttpClient httpClient, string baseAddress, string url)
        {
            this.httpClient = httpClient;
            _baseAddress = baseAddress;
            _url = url;
        }

        public RepositorioBaseAsyncR(IHttpsClientHandlerService service, string baseAddress, string url)
        {
#if DEBUG
           
            _httpsClientHandlerService = service;
            HttpMessageHandler handler = _httpsClientHandlerService.GetPlatformMessageHandler();

            if (handler != null)
                httpClient = new HttpClient(handler);
            else
                httpClient = new HttpClient();
#else
            httpClient = new HttpClient();
#endif
         
            _baseAddress = baseAddress;
            _url = url;
        }


        public async Task<bool> DeleteAsync(int id)
        {
            Uri uri = new Uri(_baseAddress+_url + "/" + id.ToString());
            HttpResponseMessage response = await httpClient.DeleteAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }

        public void Dispose()
        {

        }

        public async Task<List<T>> FindAllAsync()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            Uri uri = new Uri(_baseAddress + _url );
            HttpResponseMessage response = await httpClient.GetAsync(uri);
            if (response != null && response.IsSuccessStatusCode)
            {
                var listResponse = await response.Content.ReadAsStringAsync();
                if (listResponse != null)
                    return JsonSerializer.Deserialize<List<T>>(listResponse, options);
                else return new List<T>();
            }
            return null;
        }

        public async Task<List<T>> FindAllPagingAsync(int numPagina, int sizePage)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            UriBuilder builder = new UriBuilder(_baseAddress+_url);
            Uri uri = new Uri(_baseAddress + _url);

            builder.Query = "Numpagina=" + numPagina.ToString() + "&" + "SizePagina=" + sizePage.ToString();


            var response = await httpClient.GetAsync(builder.Uri);

            if (response != null && response.IsSuccessStatusCode)

            {
                var listResponse = await response.Content.ReadAsStreamAsync();
                if (listResponse != null)
                {
                    var lista = JsonSerializer.Deserialize<List<T>>(listResponse, options);
                    return lista;
                }
                else return new List<T>();
            }
            return null;
        }


        public async Task<T> GetAsync(int? id)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            Uri uri = new Uri(_baseAddress + _url + "/" + id.ToString());
            HttpResponseMessage response = await httpClient.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                var listResponse = await response.Content.ReadAsStringAsync();
                if (listResponse != null)
                    return JsonSerializer.Deserialize<T>(listResponse, options);
                else return null;
            }
            return null;
        }

        public async Task<List<T>> GetFilterAsync(Expression<Func<T, bool>> predicate)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            Func<T, bool> filter = predicate.Compile();

            try
            {
                Uri uri = new Uri(_baseAddress + _url );
                HttpResponseMessage response = await httpClient.GetAsync(uri);
                if (response != null && response.IsSuccessStatusCode)
                {
                    var listResponse = await response.Content.ReadAsStringAsync();
                    if (listResponse != null)
                    {
                        var lista = JsonSerializer.Deserialize<List<T>>(listResponse, options);
                        if (lista != null)
                        {
                            var rest = lista.Where(x => filter(x)).ToList();
                            return rest;
                        }
                    }
                    else return new List<T>();
                }
            }
            catch (Exception ex)
            {
                var message = ex;
            }
            return null;
        }

        public async Task<T> InsertAsync(T entidad)
        {
            T result = default;
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };


            var postData = JsonSerializer.Serialize(entidad);
            StringContent ss = new StringContent(postData, Encoding.UTF8, "application/json");
            Uri uri = new Uri(_baseAddress + _url);
            var response = await httpClient.PostAsync(uri, ss);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                result = JsonSerializer.Deserialize<T>(content, options);
            }
            return result;
        }

        public async Task<T> UpdateAsync(T entidad)
        {
            T result = default;
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            string id = (entidad as dynamic).Id.ToString();

            Uri uri = new Uri(_baseAddress + _url+  "/" + id);
            var putData = JsonSerializer.Serialize(entidad);
            StringContent ss = new StringContent(putData, Encoding.UTF8, "application/json");
            var response = await httpClient.PutAsync(uri, ss);
            if (response.IsSuccessStatusCode)
            {
                return result;
            }
            return null;

        }
    }
}



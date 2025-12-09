using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Genericos
{
    public class HttpService
    {
        private readonly HttpClient _httpClient;
        public HttpService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
    }

}

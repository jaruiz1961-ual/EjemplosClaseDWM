using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BlazorEbi7.Data;
using BlazorEbi7.Model;


namespace BlazorEbi7.API.Controllers
{
    [Route("Index")]
    [ApiController]
    public class IndexController : ControllerBase
    {

        public IndexController()
        {

        }

        [HttpGet]
        public async Task<string> Get()
        {
            var users = @"Well come to the API/Users";
            return users;
        }

      


    }
}
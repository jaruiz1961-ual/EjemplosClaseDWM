using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BlazorEbi9.Data;
using BlazorEbi9.Model;


namespace BlazorEbi9.API.Controllers
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
            var users = @"Wellcome to the API/Users";
            return users;
        }

      


    }
}
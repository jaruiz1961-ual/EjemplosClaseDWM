using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using BlazorEbi9.Data;
using BlazorEbi9.Data.DataBase;
using BlazorEbi9.Model.Entidades;
using BlazorEbi9.Model.IServices;


//https://localhost:5001/api/users

namespace BlazorEbi9.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        //private readonly IUnitOfWorkAsync _uow;
        private readonly IUsuarioServiceAsync _userService;
        public UsersController(/*IUnitOfWorkAsync uow,*/ IUsuarioServiceAsync userService)
        {
            //this._uow = uow;
            this._userService = userService;
        }

        [HttpGet]
        public async Task<List<UsuarioSet>> GetAllAsync()
        {

                var users = await _userService.FindAllAsync();
                return users;

        }

        
        [HttpPost]
        public async Task<ActionResult<UsuarioSet>> CreateAsync(UsuarioSet users)
        {
      
            if (ModelState.IsValid)
            {

                return await _userService.SaveUserAsync(users);
            }
            else
                return new BadRequestObjectResult("Error in adding user!");

        }

        [HttpGet("{Id}")]
        public async Task<UsuarioSet> GetAsync(int Id)
        {
            UsuarioSet model = null;
            if (Id > 0)
                model = await _userService.FindIdAsync(Id);
            return model;
        }

        [HttpPut("{Id}")]
        public async Task<ActionResult<bool>> Update(int Id, UsuarioSet users)
        {
            var result = false;
            if (ModelState.IsValid)
                return await _userService.SaveUserAsync(users)!=null;
            else
            return new BadRequestObjectResult("Error in updating user!");
        }

        [HttpDelete("{Id}")]
        public async Task<IActionResult> Delete([FromRoute] int Id)
        {
            UsuarioSet model = null;
            bool result = false;
            if (Id > 0)
                model = await _userService.FindIdAsync(Id);

            if (model != null)
                result = await _userService.DeleteIdAsync(Id);
            else
                return new BadRequestObjectResult("User doest not exist!");

            return new BadRequestObjectResult("Error in deleting user!");
        }


    }
}
using DataBase.Modelo;
using DataBase.Servicios;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Componentes
{
    public class UsuarioComponente<TContext> where TContext : DbContext
    {
        private readonly ServicioUsuarios<TContext> _userDataService;

        public UsuarioComponente(ServicioUsuarios<TContext> userDataService)
        {
            _userDataService = userDataService;
        }

        public async Task CrearUsuario(string nombre)
        {
            var user = new Usuario { Nombre = nombre };
            await _userDataService.AddAsync(user);
        }

        public Task<List<Usuario>> ListarUsuarios() => _userDataService.GetAllAsync();
    }

}

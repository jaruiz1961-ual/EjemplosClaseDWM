using BlazorSeguridad2026.Base.Modelo;
using BlazorSeguridad2026.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorSeguridad2026.Data.Modelo
{
    public class Usuario: Entidad, IUpdatableFrom<Usuario>
    {
        public string Codigo { get; set; }
        public string UserName { get; set; }

        public string Contexto { get; set; }
        public string Password { get; set; }
        public string? Email { get; set; }


        public void UpdateFromModel(UsuarioModel source)
        {
            // Copias solo los campos actualizables
            UserName = source.UserName;
            Password = source.Password;
            Contexto = source.Contexto;
            Email = source.Email;
            Codigo = source.Codigo;

            // No tocas Id ni TenantId aquí
        }
        public void UpdateFrom(Usuario source)
        {
            // Copias solo los campos actualizables
            UserName = source.UserName;
            Password = source.Password;
            Contexto = source.Contexto;
            Email = source.Email;
            Codigo = source.Codigo;

            // No tocas Id ni TenantId aquí
        }
        public UsuarioModel GetModel()
        {
            return new UsuarioModel
            {
                UserName = this.UserName,
                Password = this.Password,
                Contexto = this.Contexto,
                Email = this.Email??"",
                Codigo = this.Codigo,
                TenantId = this.TenantId??0
            };

        }

   
    }
}

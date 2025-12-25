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

        [Required(ErrorMessage = "Name is required")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Name cannot have less than 3 characters and more than 20 characters in length")]
        public string UserName { get; set; }

        public string Contexto { get; set; }
        public string Password { get; set; }
        public string? email { get; set; }

        public void UpdateFrom(Usuario source)
        {
            // Copias solo los campos actualizables
            UserName = source.UserName;
            Password = source.Password;
            Contexto = source.Contexto;
            email = source.email;
            Codigo = source.Codigo;

            // No tocas Id ni TenantId aquí
        }
    }
}

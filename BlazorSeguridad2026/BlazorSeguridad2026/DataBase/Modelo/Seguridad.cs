using Shares.Genericos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shares.Modelo
{
    public class Seguridad: Entidad, IUpdatableFrom<Seguridad>
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string? Email { get; set; }
        public string Roles { get; set; }
        public void UpdateFrom(Seguridad source)
        {
            // Copias solo los campos actualizables
            UserName = source.UserName;
            Password = source.Password;
            Email = source.Email;
            Roles = source.Roles;
            //TenantId = source.TenantId;
            // No tocas Id ni TenantId aquí
        }
    }
}

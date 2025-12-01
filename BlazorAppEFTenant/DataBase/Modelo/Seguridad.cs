using DataBase.Genericos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Modelo
{
    public class Seguridad: Entidad
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string? email { get; set; }
    }
}

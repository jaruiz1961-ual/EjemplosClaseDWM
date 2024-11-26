using System.ComponentModel.DataAnnotations;

namespace BlazorEbi7.Model.Entidades
{
    public partial class UsuarioSet
    {
        public int Id { get; set; }
        public string Codigo { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Name cannot have less than 3 characters and more than 20 characters in length")]
        public string UserName { get; set; }
        public int NivelAcceso { get; set; }
        public string Password { get; set; }
        public string? email { get; set; }
    }

}
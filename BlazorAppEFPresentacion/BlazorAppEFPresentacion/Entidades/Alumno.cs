namespace BlazorAppEFPresentacion.Entidades
{
    public partial class Alumno
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Apellidos { get; set; } = null!;
        public string EmailUal { get; set; } = null!;
        public string UsernameUal { get; set; } = null!;
        public string? Telefono { get; set; }
        public string? Dni { get; set; }
        public virtual ICollection<AlumnoAsignatura> AlumnosAsignaturas { get; set; } = new List<AlumnoAsignatura>();
    }
}


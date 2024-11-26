namespace BlazorAppEFPresentacion.Entidades
{
    public partial class Asignatura
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Clave { get; set; }
        public virtual ICollection<AlumnoAsignatura> AlumnosAsignaturas { get; set; } = new List<AlumnoAsignatura>();
    }

}

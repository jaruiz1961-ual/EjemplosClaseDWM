using System.Security.Cryptography.Xml;

namespace BlazorAppEF.Entidades
{
    public partial class AlumnoAsignatura
    {
        public int Id { get; set; }
        public int AlumnoId { get; set; }
        public int AsignaturaId { get; set; }
        public string? CursoAcademico { get; set; }
        public virtual Alumno Alumno { get; set; } = null!;
        public virtual Asignatura Asignatura { get; set; } = null!;
    }

}

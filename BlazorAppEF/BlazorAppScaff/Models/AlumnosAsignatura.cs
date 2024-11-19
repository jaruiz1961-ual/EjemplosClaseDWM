using System;
using System.Collections.Generic;

namespace BlazorAppScaff.Models;

public partial class AlumnosAsignatura
{
    public int Id { get; set; }

    public int AlumnosId { get; set; }

    public int AsignaturaId { get; set; }

    public string? CursoAcademico { get; set; }

    public virtual Alumno Alumnos { get; set; } = null!;

    public virtual Asignatura Asignatura { get; set; } = null!;
}

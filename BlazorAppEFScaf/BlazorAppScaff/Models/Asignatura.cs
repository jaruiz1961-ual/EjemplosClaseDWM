using System;
using System.Collections.Generic;

namespace BlazorAppScaff.Models;

public partial class Asignatura
{
    public int Id { get; set; }

    public string? Nombre { get; set; }

    public string? Clave { get; set; }

    public virtual ICollection<AlumnosAsignatura> AlumnosAsignaturas { get; set; } = new List<AlumnosAsignatura>();
}

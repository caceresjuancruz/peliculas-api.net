using System;
using System.Collections.Generic;

namespace PeliculasApi.Models;

public partial class Pelicula
{
    public int IdPelicula { get; set; }

    public string Titulo { get; set; } = null!;

    public byte Calificacion { get; set; }
}

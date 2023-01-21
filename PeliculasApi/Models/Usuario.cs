using System;
using System.Collections.Generic;

namespace PeliculasApi.Models;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public string Correo { get; set; } = null!;

    public string Clave { get; set; } = null!;
}

﻿namespace WebApp.Models.Dtos
{
    public class UsuarioActualizarDto
    {
        public int IdUsuario { get; set; } 
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public string Rol { get; set; }
        public string Clave { get; set; }
    }
}
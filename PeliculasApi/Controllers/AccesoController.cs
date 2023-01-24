using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using PeliculasApi.Models;
using System.Security.Cryptography;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Web;
using Newtonsoft.Json;

namespace PeliculasApi.Controllers
{
    public class AccesoController : Controller
    {
        static string dbConnectionString = "server=.\\SQLEXPRESS;database=PELICULASAPI;integrated security=true;encrypt=false;";
        

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Registrar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Registrar(Usuario oUsuario)
        {
            bool registrado;
            string mensaje;

            if(oUsuario.Clave == oUsuario.ConfirmarClave)
            {
                oUsuario.Clave = ConvertirSha256(oUsuario.Clave);
            }
            else
            {
                ViewData["Mensaje"] = "Las contraseñas no coinciden";
                return View();
            }

            using(SqlConnection connection = new SqlConnection(dbConnectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_RegistrarUsuario", connection);
                cmd.Parameters.AddWithValue("Correo", oUsuario.Correo);
                cmd.Parameters.AddWithValue("Clave", oUsuario.Clave);
                cmd.Parameters.Add("Registrado", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("Mensaje", SqlDbType.VarChar,100).Direction = ParameterDirection.Output;
                cmd.CommandType = CommandType.StoredProcedure;

                connection.Open();

                cmd.ExecuteNonQuery();

                registrado = Convert.ToBoolean(cmd.Parameters["Registrado"].Value);
                mensaje = cmd.Parameters["Mensaje"].Value.ToString();

            }

            ViewData["Mensaje"] = mensaje;

            if (registrado)
            {
                return RedirectToAction("Login", "Acceso");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public IActionResult Login(Usuario oUsuario)
        {
            oUsuario.Clave = ConvertirSha256(oUsuario.Clave);

            using (SqlConnection connection = new SqlConnection(dbConnectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_ValidarUsuario", connection);
                cmd.Parameters.AddWithValue("correo", oUsuario.Correo);
                cmd.Parameters.AddWithValue("clave", oUsuario.Clave);
                cmd.CommandType = CommandType.StoredProcedure;

                connection.Open();

                oUsuario.IdUsuario = Convert.ToInt32(cmd.ExecuteScalar().ToString());

            }

            if(oUsuario.IdUsuario != 0)
            {
                HttpContext.Session.SetString("usuario", JsonConvert.SerializeObject(oUsuario));
                return RedirectToAction("Index", "Home");

            }
            else
            {
                ViewData["Mensaje"] = "Usuario no encontrado";
                return View();
            }

        }

        public static string ConvertirSha256(string texto)
        {
            StringBuilder stringBuilder = new StringBuilder();
            using(SHA256 hash = SHA256Managed.Create())
            {
                Encoding encoding = Encoding.UTF8;
                byte[] result = hash.ComputeHash(encoding.GetBytes(texto));

                foreach(byte b 
                    in result)
                {
                    stringBuilder.Append(b.ToString("x2"));
                }
            }
            return stringBuilder.ToString();
        }
    }   
}

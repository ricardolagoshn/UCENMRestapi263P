using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCENMRestapi263P.Models;

namespace UCENMRestapi263P.Controllers
{
    public class EmpleadoController
    {
        private static readonly HttpClient httpClient = new HttpClient();
        
        private const string BaseUrl = "http://192.168.40.176/Crud-PHP/listaempleados.php"; 

        public async Task<List<Models.Empleado>> GetEmpleadosAsync()
        {
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(BaseUrl);
                response.EnsureSuccessStatusCode();
                string jsonResponse = await response.Content.ReadAsStringAsync();
                var empleadoResponse = System.Text.Json.JsonSerializer.Deserialize<EmpleadoResponse>(jsonResponse);
                return empleadoResponse?.Empleado ?? new List<Empleado>();
            }
            catch (Exception ex)
            {
                // Manejar errores de la solicitud HTTP
                Console.WriteLine($"Error al obtener empleados: {ex.Message}");
                return new List<Models.Empleado>();
            }
        }

        private const string CrearUrl = "http://192.168.40.176/Crud-PHP/crearempleado.php";

        // Envia el empleado a crearempleado.php (POST JSON: nombre, apellidos, edad, foto en base64).
        // Devuelve si se creo y el mensaje que respondio el servidor.
        public async Task<(bool Exito, string Mensaje)> CrearEmpleadoAsync(Empleado empleado)
        {
            try
            {
                var datos = new
                {
                    nombre = empleado.Nombre,
                    apellidos = empleado.Apellidos,
                    edad = empleado.Edad,
                    foto = empleado.Foto ?? string.Empty
                };

                string json = System.Text.Json.JsonSerializer.Serialize(datos);
                using var contenido = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await httpClient.PostAsync(CrearUrl, contenido);
                string respuesta = await response.Content.ReadAsStringAsync();

                string mensaje = respuesta;
                try
                {
                    using var doc = System.Text.Json.JsonDocument.Parse(respuesta);
                    if (doc.RootElement.TryGetProperty("message", out var msg))
                        mensaje = msg.GetString() ?? respuesta;
                }
                catch (System.Text.Json.JsonException) { }

                return (response.IsSuccessStatusCode, mensaje);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear empleado: {ex.Message}");
                return (false, ex.Message);
            }
        }

    }
}

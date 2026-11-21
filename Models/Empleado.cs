using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;


namespace UCENMRestapi263P.Models
{
    public class Empleado
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [JsonPropertyName("apellidos")]
        public string Apellidos { get; set; } = string.Empty;

        [JsonPropertyName("edad")]
        public int Edad { get; set; }

        [JsonPropertyName("foto")]
        public string? Foto { get; set; } = string.Empty;

        [JsonIgnore]
        public string NombreCompleto => $"{Nombre} {Apellidos}";

        [JsonIgnore]
        public ImageSource? FotoImageSource { get; private set; } = null;

    }
}

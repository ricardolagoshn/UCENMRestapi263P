using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace UCENMRestapi263P.Models
{
    public class EmpleadoResponse
    {
        [JsonPropertyName("empleado")]
        public List<Empleado> Empleado { get; set; } = new();
    }
}

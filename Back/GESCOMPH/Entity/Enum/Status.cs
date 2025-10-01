using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Enum
{
    public enum Status 
    {
        Pendiente = 1,
        Asignada = 2,
        Aprobada = 3,   // Generó contrato
        Finalizada = 4, // Terminó sin contrato
        Rechazada = 5
    }
}

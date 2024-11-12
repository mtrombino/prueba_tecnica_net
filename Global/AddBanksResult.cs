using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Global;

public class AddBanksResult
{
    /// <summary>
    /// Contador de bancos ingresados correctamente
    /// </summary>
    public int SuccessCount { get; set; }

    /// <summary>
    /// Lista de errores al insertar bancos
    /// </summary>
    public List<string> Errors { get; set; } = new List<string>();

}

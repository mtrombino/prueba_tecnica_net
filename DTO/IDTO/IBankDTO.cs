using System.ComponentModel.DataAnnotations;


namespace DTO.IDTO;

public interface IBankDTO
{
    /// <summary>
    /// Identificador del Banco
    /// </summary>
    int Id { get; set; }

    /// <summary>
    /// Nombre del Banco
    /// </summary>
    string Name { get; set; }

    /// <summary>
    /// BIC del Banco
    /// </summary>
    string Bic { get; set; }

    /// <summary>
    /// Pais del Banco
    /// </summary>
    string Country { get; set; }
}

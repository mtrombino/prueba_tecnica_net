using DTO.IDTO;
using System.ComponentModel.DataAnnotations;


namespace DTO.DTO;

public class BankDTO : IBankDTO
{
    /// <summary>
    /// Identificador del Banco
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nombre del Banco
    /// </summary>
    [Required(ErrorMessage = "El nombre del banco es obligatorio")]
    [MaxLength(100, ErrorMessage = "El nombre del banco no puede exceder los 100 caracteres")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// BIC del Banco
    /// </summary>
    [Required(ErrorMessage = "El código BIC es obligatorio")]
    [MaxLength(11, ErrorMessage = "El código BIC no puede exceder los 11 caracteres")]
    public string Bic { get; set; } = string.Empty;

    /// <summary>
    /// Pais del Banco
    /// La validación de la longitud del string se realiza en el servicio
    /// </summary>
    [Required(ErrorMessage = "El país es obligatorio")]
    public string Country { get; set; } = string.Empty;
}

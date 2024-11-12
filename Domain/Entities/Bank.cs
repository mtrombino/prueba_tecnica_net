using Domain.IEntities;
using System.ComponentModel.DataAnnotations;


namespace Domain.Entities;

public class Bank : IBank
{
    /// <summary>
    /// Identificador del Banco
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nombre del Banco
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// BIC del Banco
    /// </summary>
    public string Bic { get; set; } = string.Empty;

    /// <summary>
    /// Pais del Banco
    /// </summary>
    public string Country { get; set; } = string.Empty;




}

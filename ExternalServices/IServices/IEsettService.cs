using DTO.IDTO;


namespace ExternalServices.IServices;


/// <summary>
/// Clase que canaliza las llamadas a APIs externas
/// </summary>
public interface IEsettService
{
    /// <summary>
    /// Obtiene una lista de bancos desde la api externa
    /// </summary>
    Task<IEnumerable<IBankDTO>> GetBanksAsync();
}

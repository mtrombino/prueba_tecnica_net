using DTO.DTO;
using DTO.IDTO;
using Global;


namespace Services.IServices;

public interface IBankService
{
    /// <summary>
    /// Obtiene un listado de Bancos de la API externa, invocando al servicio que canaliza
    /// las llamadas a APIs externas
    /// </summary>
    Task<IEnumerable<IBankDTO>> GetBanksAsync();

    /// <summary>
    /// Agrega una lista de bancos a la base de datos evitando duplicados
    /// </summary>
    Task<AddBanksResult> AddBankListAsync(IEnumerable<BankDTO> bank);

    /// <summary>
    /// Obtiene una lista de bancos desde la base de datos
    /// </summary>
    Task<IEnumerable<IBankDTO>> GetBanksFromDatabaseAsync();

    /// <summary>
    /// Obtener un Banco por su Id
    /// </summary>
    Task<IBankDTO?> GetBankById(int id);
}

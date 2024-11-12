using Domain.Entities;


namespace Domain.IRepository;

public interface IRepositoryBank
{
    /// <summary>
    /// Agrega una lista de bancos a la base de datos evitando duplicados
    /// </summary>
    Task AddBankListAsync(IEnumerable<Bank> banks);

    /// <summary>
    /// Obtiene una lista de bancos desde la base de datos
    /// </summary>
    Task<IEnumerable<Bank>> GetAllBanksFromDataBaseAsync();

    /// <summary>
    /// Obtener un Banco por su Id
    /// </summary>
    Task<Bank?> GetBankById(int id);

}

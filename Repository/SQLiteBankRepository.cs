using Domain.Entities;
using Domain.IRepository;
using Microsoft.EntityFrameworkCore;


namespace Repository;

public class SQLiteBankRepository : IRepositoryBank
{

    private readonly MyDbContext _context;

    /// <summary>
    /// Constructor
    /// </summary>
    public SQLiteBankRepository(MyDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Agrega una lista de bancos a la base de datos evitando duplicados
    /// </summary>
    public async Task AddBankListAsync(IEnumerable<Bank> banks)
    {
        foreach (var bank in banks)
        {
            // Verifica si el banco ya existe en la base de datos
            bool exists = await _context.Banks.AnyAsync(b =>
                b.Name == bank.Name &&
                b.Bic == bank.Bic &&
                b.Country == bank.Country);

            // Si el banco no existe, lo agrega
            if (!exists)
            {
                await _context.Banks.AddAsync(bank);
            }
        }

        await _context.SaveChangesAsync();
    }


    /// <summary>
    /// Obtiene una lista de bancos desde la base de datos
    /// </summary>
    public async Task<IEnumerable<Bank>> GetAllBanksFromDataBaseAsync()
    {
        return await _context.Banks.AsNoTracking().ToListAsync();
    }


    /// <summary>
    /// Obtener un Banco por su Id
    /// </summary>
    public async Task<Bank?> GetBankById(int id)
    {        
        return await _context.Banks.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id);

    }

}

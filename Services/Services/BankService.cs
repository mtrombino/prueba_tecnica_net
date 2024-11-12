using Domain.Entities;
using Domain.IRepository;
using DTO.DTO;
using DTO.IDTO;
using ExternalServices.IServices;
using Global;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Logging;
using Services.IServices;
using System.Text.Json;


namespace Services.Services;

public class BankService : IBankService
{
    /// <summary>
    /// Logger para guardar informacion
    /// </summary>
    private readonly ILogger<BankService> _logger;

    /// <summary>
    /// Servicio que gestiona las llamadas a una API externa
    /// </summary>
    private readonly IEsettService _esettService;

    /// <summary>
    /// Repositorio de Banco
    /// </summary>
    private readonly IRepositoryBank _repositoryBank;


    /// <summary>
    /// Constructor
    /// </summary>
    public BankService(IEsettService esettService,
                       IRepositoryBank repositoryBank,
                       ILogger<BankService> logger) 
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _esettService = esettService;    
        _repositoryBank = repositoryBank;
    }


    /// <summary>
    /// Obtiene un listado de Bancos de la API externa, invocando al servicio que canaliza
    /// las llamadas a APIs externas
    /// </summary>
    public async Task<IEnumerable<IBankDTO>> GetBanksAsync()
    {
        try
        {
            return await _esettService.GetBanksAsync();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error al conectar con la API externa");
            throw new InvalidOperationException("Error al conectar con la API externa de bancos.");
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error al procesar los datos recibidos de la API externa");
            throw new InvalidOperationException("Error al procesar los datos de la API externa de bancos.");
        }
    }

    /// <summary>
    /// Agrega una lista de bancos a la base de datos evitando duplicados
    /// </summary>
    public async Task<AddBanksResult> AddBankListAsync(IEnumerable<BankDTO> banks)
    {
        var result = new AddBanksResult();
        var validBanks = new List<Bank>();

        foreach (var bankDto in banks)
        {
            // Validación de longitud de Country
            if (bankDto.Country.Length != 2)
            {
                result.Errors.Add($"Error en banco '{bankDto.Name}': el código del país '{bankDto.Country}' no es válido (debe ser de 2 caracteres).");
                continue;
            }

            // Mapea BankDTO a entidad Bank y agrega a la lista de bancos válidos
            validBanks.Add(new Bank
            {
                Name = bankDto.Name,
                Bic = bankDto.Bic,
                Country = bankDto.Country
            });
        }

        // Inserta solo los bancos válidos
        if (validBanks.Any())
        {
            await _repositoryBank.AddBankListAsync(validBanks);
            result.SuccessCount = validBanks.Count;
        }

        return result;
    }


    /// <summary>
    /// Obtiene una lista de bancos desde la base de datos
    /// </summary>
    public async Task<IEnumerable<IBankDTO>> GetBanksFromDatabaseAsync()
    {
        var banks = await _repositoryBank.GetAllBanksFromDataBaseAsync();

        // Convierte las entidades Bank a DTOs si es necesario
        return banks.Select(bank => new BankDTO
        {
            Id = bank.Id,
            Name = bank.Name,
            Bic = bank.Bic,
            Country = bank.Country
        });
    }


    /// <summary>
    /// Obtener un Banco por su Id
    /// </summary>
    public async Task<IBankDTO?> GetBankById(int id)
    {
        try
        {
            var bank = await _repositoryBank.GetBankById(id);

            if (bank == null)
            {
                return null;
            }

            return new BankDTO
            {
                Id = bank.Id,
                Name = bank.Name,
                Bic = bank.Bic,
                Country = bank.Country
            };
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogError(ex, $"Error de concurrencia al intentar obtener el banco con ID {id}");
            throw new InvalidOperationException($"Error al intentar acceder al banco con ID {id}.");
        }

    }


}

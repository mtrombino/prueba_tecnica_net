using DTO.DTO;
using DTO.IDTO;
using Microsoft.AspNetCore.Mvc;
using Services.IServices;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BankController : ControllerBase
{
    /// <summary>
    /// Servicio de Banco
    /// </summary>
    private readonly IBankService _bankService;


    /// <summary>
    /// Constructor
    /// </summary>
    public BankController(IBankService bankService)
    {
        _bankService = bankService;
    }

    /// <summary>
    /// Obtiene una lista de bancos desde la base de datos
    /// </summary>
    [HttpGet]
    [Route("GetBanksFromDatabase")]
    public async Task<ActionResult<IEnumerable<IBankDTO>>> GetBanksFromDataBase()
    {
        var banks = await _bankService.GetBanksFromDatabaseAsync();
        return Ok(banks);
    }


    /// <summary>
    /// Obtiene una lista de bancos desde la API externa
    /// </summary>
    [HttpGet]
    [Route("GetBanksFromExternalService")]
    public async Task<ActionResult<IEnumerable<IBankDTO>>> GetBanksFromExternalService()
    {
        var banks = await _bankService.GetBanksAsync();
        return Ok(banks);
    }



    /// <summary>
    /// Agrega una lista de bancos a la base de datos evitando duplicados
    /// </summary>
    [HttpPost]
    [Route("AddBankListAsync")]
    public async Task<ActionResult> AddBankListAsync([FromBody] IEnumerable<BankDTO> banks)
    {
        if (banks == null || !banks.Any())
        {
            return BadRequest("La lista de bancos está vacía o es nula.");
        }

        var result = await _bankService.AddBankListAsync(banks);

        var responseMessage = $"Se guardaron {result.SuccessCount} bancos exitosamente.";
        if (result.Errors.Any())
        {
            responseMessage += $" Hubo {result.Errors.Count} errores:";
            foreach (var error in result.Errors)
            {
                responseMessage += $"\n - {error}";
            }
        }

        return Ok(responseMessage);
    }



    /// <summary>
    /// Obtener un Banco por su Id
    /// </summary>
    [HttpGet]
    [Route("GetBankById/{id}")]
    public async Task<ActionResult<IBankDTO>> GetBankById(int id)
    {
        var bank = await _bankService.GetBankById(id);

        if (bank == null)
        {
            return NotFound();
        }

        return Ok(bank);
    }

}

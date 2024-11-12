using DTO.DTO;
using DTO.IDTO;
using ExternalServices.IServices;
using System.Text.Json;

namespace ExternalServices.Services;


/// <summary>
/// Clase que canaliza las llamadas a APIs externas
/// </summary>
public class EsettService : IEsettService
{
    private readonly HttpClient _httpClient;


    /// <summary>
    /// Constructor
    /// </summary>
    public EsettService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }


    /// <summary>
    /// Obtiene una lista de bancos desde la api externa
    /// Para simplificar el ejercicio se coloca la url en el metodo en lugar de un archivo
    /// de configuración aparte
    /// </summary>
    public async Task<IEnumerable<IBankDTO>> GetBanksAsync()
    {
        var response = await _httpClient.GetAsync("https://api.opendata.esett.com/EXP06/Banks");
        response.EnsureSuccessStatusCode();

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var json = await response.Content.ReadAsStringAsync();
        var banks = JsonSerializer.Deserialize<List<BankDTO>>(json, options) ?? new List<BankDTO>();

        return banks;
    }
}

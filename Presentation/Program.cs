using DTO.DTO;
using DTO.IDTO;
using Microsoft.Extensions.Configuration;

using System.Text.Json;


class Program
{
    static void Main(string[] args)
    {
        RunAsync().GetAwaiter().GetResult();
    }


    private static async Task RunAsync()
    {
        // Inicio
        Console.WriteLine("Inicio " + DateTime.Now.ToString("dd/MM/yyyy - HH:mm:ss"));
        Console.WriteLine();

        // Cargar configuración desde el appsettings.json
        var configuration = LoadConfiguration();

        // Construir la ruta absoluta de la base de datos
        var dbPath = GetDatabasePath(configuration);
        if (dbPath == null)
        {
            Console.WriteLine("Error: No se pudo encontrar el archivo de base de datos.");
            Exit();
            return;
        }

        // Obtener bancos
        var apiBaseUrl = configuration["ApiSettings:BankApiBaseUrl"];

        if (string.IsNullOrEmpty(apiBaseUrl))
        {
            Console.WriteLine("Error: La URL de la API no está configurada en appsettings.json.");
            Exit();
            return;
        }

        var banks = await GetBanksFromApi(apiBaseUrl);

        if (banks == null || !banks.Any())
        {
            Console.WriteLine("No se han encontrado bancos disponibles para continuar el ejercicio.");
            Exit();
            return;
        }

        Console.WriteLine($"Se encontraron: {banks.Count()} bancos");
        Console.WriteLine("\nGuardando en base de datos.\n");

        // Llamar a AddBanksToDatabaseViaApi y mostrar el mensaje directamente
        await AddBanksToDatabaseViaApi(apiBaseUrl, banks);

        Exit();
    }




    /// <summary>
    /// Método para cargar la configuración del appsettings
    /// </summary>
    private static IConfiguration LoadConfiguration()
    {
        return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
    }


    /// <summary>
    /// Método para construir la ruta absoluta de la base de datos
    /// para simplificar el acceso al archivo de la base de datos para el ejercicio
    /// </summary>
    private static string? GetDatabasePath(IConfiguration configuration)
    {
        var relativePath = configuration.GetConnectionString("SQLiteConnection")?.Replace("Data Source=", "");
        var solutionRoot = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.Parent?.FullName;

        if (solutionRoot == null || string.IsNullOrEmpty(relativePath))
        {
            return null;
        }

        var dbPath = Path.Combine(solutionRoot, relativePath);

        if (!File.Exists(dbPath))
        {
            Console.WriteLine($"Error: El archivo de base de datos no se encontró en la ruta especificada: {dbPath}");
            return null;
        }

        return dbPath;
    }




    /// <summary>
    /// Metodo para salir de la aplicación al terminar su ejecución
    /// </summary>
    private static void Exit()
    {
        Console.WriteLine();
        Console.WriteLine("Proceso finalizado. Presione una tecla para salir.");
        Console.ReadKey();
    }



    /// <summary>
    /// Llama al endpoint de la API para obtener la lista de bancos
    /// </summary>
    private static async Task<IEnumerable<IBankDTO>?> GetBanksFromApi(string urlApi)
    {
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(urlApi);

        Console.WriteLine("Leyendo la API interna - Servicio Bank");
        var response = await httpClient.GetAsync("GetBanksFromExternalService");

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine("Error al obtener bancos desde la API.");
            return null;
        }

        var json = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var banks = JsonSerializer.Deserialize<List<BankDTO>>(json, options);
        return banks;
    }



    /// <summary>
    /// Agrega una lista de bancos a la base de datos
    /// </summary>
    private static async Task AddBanksToDatabaseViaApi(string apiBaseUrl, IEnumerable<IBankDTO> banks)
    {
        using var httpClient = new HttpClient { BaseAddress = new Uri(apiBaseUrl) };

        var jsonData = JsonSerializer.Serialize(banks);
        var content = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync("AddBankListAsync", content);

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Error al guardar los bancos en la base de datos: {response.ReasonPhrase}");
            return;
        }

        // Leer el contenido de la respuesta y mostrarlo directamente
        var responseContent = await response.Content.ReadAsStringAsync();
        Console.WriteLine(responseContent);
    }




}
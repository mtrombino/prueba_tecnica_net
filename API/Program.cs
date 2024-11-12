using Domain.IRepository;
using ExternalServices.IServices;
using ExternalServices.Services;
using Microsoft.EntityFrameworkCore;
using Repository;
using Services.IServices;
using Services.Services;

var builder = WebApplication.CreateBuilder(args);

// Cargar la configuración y registrar servicios
ConfigureServices(builder);

var app = builder.Build();

// Configurar el pipeline HTTP de la aplicación
ConfigurePipeline(app);

app.Run();

// Método para configurar los servicios
void ConfigureServices(WebApplicationBuilder builder)
{
    // Configuración de controladores y Swagger
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // Registro de dependencias de servicios y repositorios
    builder.Services.AddHttpClient<IEsettService, EsettService>();
    builder.Services.AddScoped<IBankService, BankService>();
    builder.Services.AddScoped<IRepositoryBank, SQLiteBankRepository>();

    // Configurar la conexión a la base de datos
    var dbPath = GetDatabasePath(builder.Configuration);
    builder.Services.AddDbContext<MyDbContext>(options =>
        options.UseSqlite($"Data Source={dbPath}"));
}

// Método para configurar el pipeline de la aplicación
void ConfigurePipeline(WebApplication app)
{
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseMiddleware<ExceptionMiddleware>();
    app.UseAuthorization();
    app.MapControllers();
}

// Método para construir la ruta absoluta de la base de datos
string GetDatabasePath(IConfiguration configuration)
{
    var relativePath = configuration.GetConnectionString("SQLiteConnection")?.Replace("Data Source=", "");
    var solutionRoot = Directory.GetParent(Directory.GetCurrentDirectory())!.FullName;
    var dbPath = Path.Combine(solutionRoot, relativePath!);

    // Verificar si el archivo existe
    if (!File.Exists(dbPath))
    {
        Console.WriteLine($"Error: El archivo de base de datos no se encontró en la ruta especificada: {dbPath}");
        throw new FileNotFoundException("No se encontró el archivo de base de datos", dbPath);
    }

    Console.WriteLine($"Ruta completa de la base de datos: {dbPath}");
    return dbPath;
}

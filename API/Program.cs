using Domain.IRepository;
using ExternalServices.IServices;
using ExternalServices.Services;
using Microsoft.EntityFrameworkCore;
using Repository;
using Services.IServices;
using Services.Services;

var builder = WebApplication.CreateBuilder(args);

// Cargar la configuraci�n y registrar servicios
ConfigureServices(builder);

var app = builder.Build();

// Configurar el pipeline HTTP de la aplicaci�n
ConfigurePipeline(app);

app.Run();

// M�todo para configurar los servicios
void ConfigureServices(WebApplicationBuilder builder)
{
    // Configuraci�n de controladores y Swagger
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // Registro de dependencias de servicios y repositorios
    builder.Services.AddHttpClient<IEsettService, EsettService>();
    builder.Services.AddScoped<IBankService, BankService>();
    builder.Services.AddScoped<IRepositoryBank, SQLiteBankRepository>();

    // Configurar la conexi�n a la base de datos
    var dbPath = GetDatabasePath(builder.Configuration);
    builder.Services.AddDbContext<MyDbContext>(options =>
        options.UseSqlite($"Data Source={dbPath}"));
}

// M�todo para configurar el pipeline de la aplicaci�n
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

// M�todo para construir la ruta absoluta de la base de datos
string GetDatabasePath(IConfiguration configuration)
{
    var relativePath = configuration.GetConnectionString("SQLiteConnection")?.Replace("Data Source=", "");
    var solutionRoot = Directory.GetParent(Directory.GetCurrentDirectory())!.FullName;
    var dbPath = Path.Combine(solutionRoot, relativePath!);

    // Verificar si el archivo existe
    if (!File.Exists(dbPath))
    {
        Console.WriteLine($"Error: El archivo de base de datos no se encontr� en la ruta especificada: {dbPath}");
        throw new FileNotFoundException("No se encontr� el archivo de base de datos", dbPath);
    }

    Console.WriteLine($"Ruta completa de la base de datos: {dbPath}");
    return dbPath;
}

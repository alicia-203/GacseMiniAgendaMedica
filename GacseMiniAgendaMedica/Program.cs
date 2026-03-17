using GacseMiniAgendaMedica.Data;
using GacseMiniAgendaMedica.Data.Repositories;
using GacseMiniAgendaMedica.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddScoped<MedicoRepository>();
builder.Services.AddScoped<IPacienteRepository,PacienteRepository>();
builder.Services.AddScoped<ICitaRepository,CitaRepository>();

builder.Services.AddScoped<MedicoService>();
builder.Services.AddScoped<IPacienteService,PacienteService>();
builder.Services.AddScoped<ICitaService,CitaService>();

// Configurar Controllers y Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "GACSEMiniAgendaMedica API", Version = "v1" });
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "GACSEMiniAgendaMedica API v1");
    c.RoutePrefix = string.Empty; 
});

app.MapControllers();

app.Run();
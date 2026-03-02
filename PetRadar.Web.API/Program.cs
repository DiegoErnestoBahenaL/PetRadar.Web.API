using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PetRadar.Core.Data;
using PetRadar.Core.Data.Repositories;
using PetRadar.Core.Domain;
using PetRadar.Web.API;
using PetRadar.Web.API.Services;
using System.Text;
using System.Text.Json.Serialization;


var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");


var builder = WebApplication.CreateBuilder(args);

var configuration = new ConfigurationBuilder()
          .AddJsonFile("appsettings.json")
          .AddJsonFile($"appsettings.{environmentName}.json", true)
          .Build();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");


builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserPetRepository, UserPetRepository>();
builder.Services.AddScoped<IVeterinaryAppointmentRepository, VeterinaryAppointmentRepository>();
builder.Services.AddScoped<IAdoptionAnimalRepository, AdoptionAnimalRepository>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IMatchRepository, MatchRepository>();

builder.Services.AddScoped<IUserDomain, UserDomain>();
builder.Services.AddScoped<IUserPetDomain, UserPetDomain>();
builder.Services.AddScoped<IVeterinaryAppointmentDomain, VeterinaryAppointmentDomain>();
builder.Services.AddScoped<IAdoptionAnimalDomain, AdoptionAnimalDomain>();
builder.Services.AddScoped<IReportDomain, ReportDomain>();
builder.Services.AddScoped<IMatchDomain, MatchDomain>();

builder.Services.AddDbContext<PetRadarDbContext>(options =>
    options.UseNpgsql(connectionString, x => x.MigrationsAssembly(Constants.MigrationsAssembly)
    .UseNetTopologySuite()
    ));


builder.Services.AddAuthentication(auth => {
    auth.DefaultAuthenticateScheme =  JwtBearerDefaults.AuthenticationScheme;
    auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "petradar.com",
        ValidAudience = "petradar.com",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("RedactedRedactedRedactedRedacted"))
    };
});
builder.Services.AddSingleton<IJwtHelper, JwtHelper>();

builder.Services.AddHttpContextAccessor();

// Add services to the container.
builder.Services.AddHealthChecks();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowEverything",
        builder =>
        {
            builder.WithOrigins("*") 
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<PetRadarDbContext>();
    dbContext.Database.Migrate();
}


// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("AllowEverything");

app.UseSwagger();
app.UseSwaggerUI();


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/api/health");

app.Run();


// This public partial class is used in order to make integration testing possible
// by implicitly exposing the Program class
// Reference: https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-6.0
public partial class Program { }


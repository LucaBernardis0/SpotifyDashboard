using MongoDB.Driver;
using SpotifyDashboard.Server.Endpoints;
using SpotifyDashboard.Server.Models.Dashboard;
using SpotifyDashboard.Server.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<DashboardService>();
builder.Services.AddScoped<ConfigService>();
builder.Services.AddCors();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IMongoClient>(_ => new MongoClient("mongodb://localhost:27017/")); // Add connection string to the mongodb

var app = builder.Build();

app.UseHttpsRedirection();
if (app.Environment.IsDevelopment())
    app.UseCors(app =>
    {
        app.AllowAnyOrigin();
        app.AllowAnyMethod();
        app.AllowAnyHeader();
    });


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Mapping created Enpoints
app.MapDashboardEndPoint(); // Quest'unico endpoint gestisce tutte le chiamate ai metodi di cui si ha bisogno per far si che la dashboard funzioni

// Passo 5
var mongoclient = app.Services.GetService<IMongoClient>();
var tiles = mongoclient.GetDatabase("Spotify").GetCollection<WidgetComponent>("Tiles");
if(tiles == null)
{
    // inserisci su db i dati dei widget
}


app.Run();

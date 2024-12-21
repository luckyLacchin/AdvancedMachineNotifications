using DemoServiceProto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Extensions.Logging;
using Telegram.Bot;
using TelegramClientServer.Bot;
using TelegramClientServer.Data;
using TelegramClientServer.Models;
using TelegramClientServer.Services;


GlobalDiagnosticsContext.Set("taskID", nameof(TelegramBotClient));


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.ClearProviders();
    loggingBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
    loggingBuilder.AddNLog();
});
var logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();
builder.Services.AddSingleton<ChannelServerApi>();

var channelServerApi = builder.Services.BuildServiceProvider().GetService<ChannelServerApi>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSingleton<IBotEngine, BotEngine>(); //metto Singleton perch� mi basta un'unica istanza del bot Telegram
builder.Services.AddGrpc();

var sqlConBuilder = new SqlConnectionStringBuilder();
sqlConBuilder.ConnectionString = builder.Configuration.GetConnectionString("mainDB");

#region MigrationDb
try
{
    using (var db = new TelegramContext())
    {
        db.Database.Migrate();
    }
}
catch (Exception ex)
{
    logger.LogError(ex, ex.Message);
    Environment.Exit(-1);
} 
#endregion


builder.Services.AddDbContext<TelegramContext>(opt => opt.UseSqlite(sqlConBuilder.ConnectionString));
builder.Services.AddScoped<IChatterRepo, ChatterRepo>();
builder.Services.AddHostedService<BotEngine>(serviceProvider => new BotEngine(
    configuration: builder.Configuration,
    _logger: serviceProvider.GetRequiredService<ILogger<BotEngine>>(),
    _channelServerApi: channelServerApi));

builder.WebHost.ConfigureKestrel(option => { });

logger.LogInformation("\n");
logger.LogInformation("-------------------------------------------------------------------");

builder.Services.AddHostedService<IsAliveTimer>(serviceProvider => new IsAliveTimer(
    _logger: serviceProvider.GetRequiredService<ILogger<IsAliveTimer>>(),
    lifetime: serviceProvider.GetRequiredService<IHostApplicationLifetime>(),
    channelServerApi: channelServerApi,
    _configuration: builder.Configuration
    ));
var app = builder.Build();
app.UseRouting();
var url = builder.Configuration.GetValue<string>("Kestrel:Endpoints:Grpc:Url");
var port = url.Split(":").Last();
app.MapGrpcService<DispatcherService>().RequireHost($"*:{port}");

app.MapGet("api/v1/bot/{mId}", async (IChatterRepo repo, int mId) =>
{

    try
    {
        var chatter = await repo.GetChatter(mId);
        if (chatter == null)
        {
            return Results.NotFound(); //� il caso in cui l'id dato � sbagliato e non ho ottenuto un sub!
        }
        return Results.Ok(chatter);

    }
    catch (Exception ex)
    {
        //in questo catch mettiamo il caso in cui il SingleOrDefault ci d� errore perch� trova pi� di un sub
        return Results.Conflict();
    }

});
app.MapPost("api/v1/bot/", async (IChatterRepo repo, TelegramContext db, Chatter created) =>
{

    try
    {
        if (created == null)
            return Results.BadRequest();
        else
        {
            var result = await repo.GetChatter(created.Id);
            if (result != null)
                return Results.BadRequest(); //caso in cui la macchina � gi� registrata
            await db.AddAsync(created);
            await repo.SaveChanges(); 
            Console.WriteLine(created.Id);
            return Results.Created($"api/v1/commands/{created.Id}", created); //la prima stringa ci restituisce diciamo la location dove abbiamo salvato il nostro sub!

        }
    }
    catch (Exception ex)
    {
        return Results.BadRequest(); //questo � il caso in cui abbiamo inserito un MId gi� presente pi� volte
    }
});


app.MapPut("api/v1/bot/", async (IChatterRepo repo, Chatter chatter) =>
{
    try
    {

        if (chatter == null)
        {
            return Results.BadRequest(); 
        }
        var chatterToUpdate = await repo.GetChatter(chatter.Id);
        if (chatterToUpdate == null)
        {
            return Results.NotFound(); //� il caso in cui l'id dato � sbagliato e non ho ottenuto un sub!
        }
        repo.updateChatter(chatterToUpdate, chatter);
        await repo.SaveChanges();
        return Results.Ok("Successful Update");

    }
    catch (Exception ex)
    {
        //in questo catch mettiamo il caso in cui il SingleOrDefault ci d� errore perch� trova pi� di un sub
        return Results.Conflict();
    }
});

app.MapDelete("api/v1/bot/{id}", async ([FromServices] IChatterRepo repo, int mId) =>
{
    try
    {
        var chatterToDelete = await repo.GetChatter(mId);
        if (chatterToDelete == null)
        {
            return Results.NotFound(); //� il caso in cui l'id dato � sbagliato e non ho ottenuto un sub! Potevo anche mettere BadRequest
        }
        repo.DeleteChatter(chatterToDelete);
        await repo.SaveChanges();
        return Results.Ok("Successful Delete");

    }
    catch (Exception ex)
    {
        //in questo catch mettiamo il caso in cui il SingleOrDefault ci d� errore perch� trova pi� di un sub
        //qua potremmo anche avere problemi per la delete 
        return Results.Conflict();
    }
});
logger.LogInformation("Starting Services telegram");

app.Run();


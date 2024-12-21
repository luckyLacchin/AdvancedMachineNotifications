
using DemoAPIBot.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NLog.Extensions.Logging;
using DemoAPIBot.Extensions;
using DemoAPIBot.Services;
using System.Text.Json.Serialization;
using FastEndpoints;
using FastEndpoints.Swagger;
using FastEndpoints.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Hosting;
using NLog;
using Microsoft.Extensions.DependencyInjection;
using DemoAPIBot;

GlobalDiagnosticsContext.Set("taskID", nameof(DemoAPIBot));

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.ClearProviders();
    loggingBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
    loggingBuilder.AddNLog();
});
var logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();

var sqlConBuilder = new SqlConnectionStringBuilder();
sqlConBuilder.ConnectionString = builder.Configuration.GetConnectionString("mainDB");


try
{
    using (var db = new DemoContext())
    {
        db.Database.Migrate();
    }
}
catch (Exception ex) { 
    logger.LogError(ex,ex.Message);
    Environment.Exit(-1);
}



logger.LogInformation("\n");
logger.LogInformation("-------------------------------------------------------------------");
builder.Services.SwaggerDocument();
builder.Services.AddDbContext<DemoContext>(opt => opt.UseSqlite(sqlConBuilder.ConnectionString));
builder.Services.AddGrpc();


builder.Services.AddScoped<ISubRepo, SubRepo>();
builder.Services.AddScoped<IMacchinaRepo, MacchinaRepo>(); 
builder.Services.AddScoped<ISubMachineRepo, SubMachineRepo>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddFastEndpoints();
builder.Services.AddJWTBearerAuth(builder.Configuration.GetValue<string>("Jwt:Key")); 
builder.Services.AddHostedService<SubTimer>(serviceProvider => new SubTimer(
    _logger: serviceProvider.GetRequiredService<ILogger<SubTimer>>(),
    lifetime: serviceProvider.GetRequiredService<IHostApplicationLifetime>()
    ));
builder.Services.AddHostedService<PingTimer>();
builder.Services.AddHostedService<RefreshTokenTimer>();
builder.WebHost.ConfigureKestrel(option => {
    option.ConfigureEndpointDefaults(opt => { });

});

var app = builder.Build();
app.UseRouting();
app.UseSwaggerAuthorize();
app.UseApiAuthorize();
EndpointParameters.parseEndpoint(builder.Configuration.GetValue<string>("Kestrel:Endpoints:Grpc:Url"), out bool https, out int grpcPort);
app.MapGrpcService<ServerService>().RequireHost($"*:{grpcPort}");
app.UseAuthentication();
app.UseAuthorization();
app.UseFastEndpoints();

if (app.Environment.IsDevelopment())
{

    app.UseSwaggerGen();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = "swagger";
    });
}
logger.LogInformation("Starting Services demoapi");
app.Run();
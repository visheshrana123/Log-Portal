using LogPortal.API.Data;
using LogPortal.API.Services;

var builder = WebApplication.CreateBuilder(args);
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Urls.Add($"http://0.0.0.0:{port}");

var mongoSettings = builder.Configuration
    .GetSection("MongoDbSettings")
    .Get<MongoDbSettings>();

if (mongoSettings == null)
    throw new Exception("MongoDbSettings is missing in appsettings.json!");


builder.Services.AddSingleton(mongoSettings);


builder.Services.AddSingleton<MongoDbContext>();


builder.Services.AddScoped<LogService>();
builder.Services.AddScoped<AnomalyDetectionService>();
builder.Services.AddScoped<AlertService>(); 


builder.Services.AddControllers();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "Log Aggregation + Anomaly Detection API",
        Version = "v1",
        Description = "Backend API for the GLA University Mini Project - Group 17"
    });
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "LogPortal API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();


app.MapGet("/health", () => new
{
    status = "running",
    project = "Log Aggregation + Anomaly Detection Portal",
    group = "Group 17 - Section 3N",
    timestamp = DateTime.UtcNow
});

app.Run();

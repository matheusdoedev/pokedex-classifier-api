using StackExchange.Redis;

WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();

// Configure Model options from configuration
builder.Services.Configure<ModelOptions>(
    builder.Configuration.GetSection(ModelOptions.SectionName));

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    string? config = builder.Configuration.GetSection("Redis")["ConnectionString"]
        ?? throw new ArgumentException("redis connection string is null");

    return ConnectionMultiplexer.Connect(config);
});

// HTTP Client for Model adapter with proper configuration
builder.Services.AddHttpClient<ModelPort, ModelAdapter>()
    .ConfigureHttpClient(client =>
    {
        var modelConfig = builder.Configuration.GetSection(ModelOptions.SectionName);
        var timeoutSeconds = modelConfig.GetValue<int?>("TimeoutSeconds") ?? 30;
        client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
    });

// #region ports & adapters
builder.Services.AddScoped<NoSqlDatabasePort, NoSqlDatabaseAdapter>();
builder.Services.AddScoped<ClassifierInterceptor, ClassifierInterceptorImpl>();
// #endregion

WebApplication? app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();
app.UseHttpsRedirection();
await app.RunAsync();

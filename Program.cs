using StackExchange.Redis;

WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    string? config = builder.Configuration.GetSection("Redis")["ConnectionString"] ?? throw new ArgumentException("redis connection string is null");

    return ConnectionMultiplexer.Connect(config);
});
// #region ports & adapters
builder.Services.AddScoped<NoSqlDatabasePort, NoSqlDatabaseAdapter>();
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
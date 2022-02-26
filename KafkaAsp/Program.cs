using System.Text.Json;
using KafkaAsp.Configuration;
using KafkaAsp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Logging.ClearProviders();
builder.Logging.AddJsonConsole(x => x.JsonWriterOptions = new JsonWriterOptions { Indented = true, });

builder.Services.Configure<KafkaAspConfig>(builder.Configuration.GetSection(nameof(KafkaAspConfig)));

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IKafkaMessageConsumer, KafkaMessageConsumer>();
builder.Services.AddSingleton<IKafkaMessageProducer, KafkaMessageProducer>();
builder.Services.AddSingleton<ICoreBankingService, CoreBankingService>();

builder.Services.AddHostedService<MoneyLaunderingService>();

builder.Services.AddSingleton<TransactionAnalyticsService>();
builder.Services.AddSingleton<ITransactionAnalyticsService>(x => x.GetRequiredService<TransactionAnalyticsService>());
builder.Services.AddHostedService(x => x.GetRequiredService<TransactionAnalyticsService>());

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

app.Run();

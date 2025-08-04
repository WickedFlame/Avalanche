using Avalanche;
using Avalanche.DataSource;
using Avalanche.Domain;
using Avalanche.ReadModel;
using Avalanche.ReadModel.QueryHandlers;
using Avalanche.ReadModel.Sql.QueryHandlers;
using Avalanche.WriteModel.Events;
using Avalanche.WriteModel.Sql;
using Avalanche.WriteModel.Sql.EventHandlers;
using Broadcast;
using OpenTelemetry.Logs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();


// transient services create a new instance every time they're requested, while scoped services create one instance per client request (or scope)
// services.AddSingleton<ExampleService>();
// services.AddTransient<ExampleService>();
// services.AddScoped<ExampleService>();

// https://www.scottbrady.io/docker/aspnet-core-and-docker-environment-variables

using var loggerFactory = LoggerFactory.Create(loggingBuilder => loggingBuilder
    .SetMinimumLevel(LogLevel.Trace)
        .AddOpenTelemetry(options =>
            options.AddConsoleExporter())
        );
var logger = loggerFactory.CreateLogger<Program>();

var config = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .Build();
builder.Services.AddSingleton(config);

var dbSource = config.GetValue<string>("AV_DB");

logger.LogInformation(!string.IsNullOrEmpty(dbSource) ? "Using configured Sqlprovider {Provider}" : "No DB Server defined. Switching to default", dbSource);

if (dbSource == "pgsql")
{
    builder.Services.AddTransient<IEventStoreConnectionBuilder, Avalanche.DataSource.Pgsql.EventStoreConnectionBuilder>();
    builder.Services.AddSingleton<IProjectionConnectionBuilder, Avalanche.DataSource.Pgsql.ProjectionConnectionBuilder>();
    builder.Services.AddSingleton<IDataStoreBuilder, Avalanche.DataSource.Pgsql.EventStoreBuilder>();
}
else
{
    builder.Services.AddTransient<IEventStoreConnectionBuilder, Avalanche.DataSource.Sqlite.EventStoreConnectionBuilder>();
    builder.Services.AddSingleton<IProjectionConnectionBuilder, Avalanche.DataSource.Sqlite.ProjectionConnectionBuilder>();
    builder.Services.AddSingleton<IDataStoreBuilder, Avalanche.DataSource.Sqlite.EventStoreBuilder>();
}

builder.Services.AddSingleton<IEventStore, SqlEventStore>();

builder.Services.AddTransient<IEventBus>(c =>
{
    var eventBus = new EventBus(c.GetService<IEventStore>());
    eventBus.Subscribe<StartTestEvent>(new TestRunEventHandler(c.GetService<IProjectionConnectionBuilder>()));
    eventBus.Subscribe<EndTestEvent>(new TestRunEventHandler(c.GetService<IProjectionConnectionBuilder>()));
    eventBus.Subscribe<ThreadSummaryEvent>(new SummaryEventHandler(c.GetService<IProjectionConnectionBuilder>()));
    eventBus.Subscribe<TestSummaryEvent>(new SummaryEventHandler(c.GetService<IProjectionConnectionBuilder>()));

    eventBus.Subscribe<RampupEvent>(new RampupEventHandler(c.GetService<IProjectionConnectionBuilder>()));
    eventBus.Subscribe<RampdownEvent>(new RampupEventHandler(c.GetService<IProjectionConnectionBuilder>()));
    eventBus.Subscribe<IterationLogEvent>(new IterationEventHandler(c.GetService<IProjectionConnectionBuilder>()));
    eventBus.Subscribe<IterationErrorEvent>(new IterationEventHandler(c.GetService<IProjectionConnectionBuilder>()));

    eventBus.Subscribe<DeleteTestRunEvent>(new DeleteTestRunEventHandler(c.GetService<IProjectionConnectionBuilder>()));

    return eventBus;
});
builder.Services.AddSingleton<ISettingsQueryHandler, SettingsQueryHandler>();
builder.Services.AddTransient<ITestRunQueryHandler, TestRunQueryHandler>();
builder.Services.AddTransient<ISettingsFacade, SettingsFacade>();

builder.Services.AddLogging((loggingBuilder) => loggingBuilder
        .SetMinimumLevel(LogLevel.Debug)
        .AddOpenTelemetry(options =>
            options.AddConsoleExporter())
        );

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{


    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseEventStore();
app.UseReadModel();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

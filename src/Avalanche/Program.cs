using Avalanche;
using Avalanche.Domain;
using Avalanche.ReadModel;
using Avalanche.ReadModel.QueryHandlers;
using Avalanche.ReadModel.Sqlite.QueryHandlers;
using Avalanche.WriteModel;
using Avalanche.WriteModel.Events;
using Avalanche.WriteModel.Sqlite;
using Avalanche.WriteModel.Sqlite.EventHandlers;
using Broadcast;
using Microsoft.AspNetCore.OpenApi;
using SqlKata.Compilers;
using SqlKata.Execution;
using System.Data.SQLite;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();


// transient services create a new instance every time they're requested, while scoped services create one instance per client request (or scope)
// services.AddSingleton<ExampleService>();
// services.AddTransient<ExampleService>();
// services.AddScoped<ExampleService>();


builder.Services.AddSingleton<IEventStore, SqliteEventStore>();
builder.Services.AddTransient<QueryFactory>(c =>
{
    var connection = new SQLiteConnection(Constants.ReadModelDatabase);
    var compiler = new SqliteCompiler();
    return new QueryFactory(connection, compiler);
});
builder.Services.AddTransient<IEventBus>(c =>
{
    var eventBus = new EventBus(c.GetService<IEventStore>());
    eventBus.Subscribe<StartTestEvent>(new TestRunEventHandler(c.GetService<QueryFactory>()));
    eventBus.Subscribe<EndTestEvent>(new TestRunEventHandler(c.GetService<QueryFactory>()));
    eventBus.Subscribe<ThreadSummaryEvent>(new SummaryEventHandler(c.GetService<QueryFactory>()));
    eventBus.Subscribe<TestSummaryEvent>(new SummaryEventHandler(c.GetService<QueryFactory>()));

    eventBus.Subscribe<RampupEvent>(new RampupEventHandler(c.GetService<QueryFactory>()));
    eventBus.Subscribe<RampdownEvent>(new RampupEventHandler(c.GetService<QueryFactory>()));
    eventBus.Subscribe<IterationLogEvent>(new IterationEventHandler(c.GetService<QueryFactory>()));
    eventBus.Subscribe<IterationErrorEvent>(new IterationEventHandler(c.GetService<QueryFactory>()));

    eventBus.Subscribe<DeleteTestRunEvent>(new DeleteTestRunEventHandler(c.GetService<QueryFactory>()));

    return eventBus;
});
builder.Services.AddSingleton<ISettingsQueryHandler, SettingsQueryHandler>();
builder.Services.AddSingleton<TestRunQueryHandler>();

builder.Services.AddTransient<ISettingsFacade, SettingsFacade>();





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

app.UseSqliteEventStore();
app.UseSqliteReadModel();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

using Avalanche;
using Avalanche.ReadModel.QueryHandlers;
using Avalanche.WriteModel;
using Avalanche.WriteModel.Events;
using Avalanche.WriteModel.Sqlite;
using Avalanche.WriteModel.Sqlite.EventHandlers;
using Broadcast;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddSingleton<IEventStore, SqliteEventStore>();
builder.Services.AddSingleton<IEventBus>(c =>
{
    var eventBus = new EventBus(c.GetService<IEventStore>());
    eventBus.Subscribe<StartTestEvent>(new TestRunEventHandler());
    eventBus.Subscribe<EndTestEvent>(new TestRunEventHandler());
    eventBus.Subscribe<ThreadSummaryEvent>(new SummaryEventHandler());
    eventBus.Subscribe<TestSummaryEvent>(new SummaryEventHandler());

    eventBus.Subscribe<RampupEvent>(new RampupEventHandler());
    eventBus.Subscribe<RampdownEvent>(new RampupEventHandler());
    eventBus.Subscribe<IterationLogEvent>(new IterationEventHandler());

    return eventBus;
});
builder.Services.AddSingleton<TestRunQueryHandler>();
//builder.Services.AddScoped<IDispatcher<ICommand>, CommandDispatcher>();





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

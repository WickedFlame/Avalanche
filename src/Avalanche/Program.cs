using Avalanche;
using Avalanche.WriteModel.Sqlite;
using Avalanche.WriteModel.Sqlite.EventHandlers;
using Avalanche.WriteModel.Events;
using Broadcast;
using Avalanche.WriteModel;
using Avalanche.WriteModel.CommandHandlers;
using Avalanche.WriteModel.Commands;
using Microsoft.AspNetCore.Components;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddSingleton<IEventStore, SqliteEventStore>();
builder.Services.AddScoped<IEventBus>(c =>
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
builder.Services.AddScoped<IDispatcher<ICommand>>(c =>
{
    var eventBus = c.GetService<IEventBus>();
    var dispatcher = new Dispatcher<ICommand>();
    dispatcher.Register<StartTestCommand>(new StartTestCommandHandler(eventBus));
    dispatcher.Register<EndTestCommand>(new EndTestCommandHandler(eventBus));
    dispatcher.Register<TestResultCommand>(new TestResultCommandHandler(eventBus));

    dispatcher.Register<StartupThreadCommand>(new StartupThreadCommandHandler(eventBus));
    dispatcher.Register<EndThreadCommand>(new EndThreadCommandHandler(eventBus));
    dispatcher.Register<IterationCommand>(new IterationCommandHandler(eventBus));

    return dispatcher;
});





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

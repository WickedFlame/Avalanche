using Avalanche.WriteModel;
using Avalanche.WriteModel.CommandHandlers;
using Avalanche.WriteModel.Commands;
using Avalanche.WriteModel.EventHandlers;
using Avalanche.WriteModel.Events;
using Broadcast;
using MeasureMap;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;
using System.Security.Cryptography;

namespace Avalanche.Runner
{
    public class LoadTest
    {
        private readonly List<IDispatcher<ICommand>> _dispatchers = [];

        private readonly string _testId;
        private readonly IEventStore _store;

        public LoadTest(string testId, IEventStore store)
        {
            _testId = testId;
            _store = store;
        }

        public IEnumerable<TestResult> Run(TestSettings settings)
        {
            var results = new List<TestResult>();

            foreach (var test in settings.Tests)
            {
                var session = ProfilerSession.StartSession()
                    .AddMiddleware(new ItterationLogCollectionTaskHandler(test.Name, _testId))
                    .OnStartPipeline(s =>
                    {
                        var ctx = new MeasureMap.ExecutionContext(s);

                        var clientHandler = new HttpClientHandler
                        {
                            AllowAutoRedirect = true,
                            UseCookies = test.UseCookies,
                            CookieContainer = new CookieContainer(),
                            ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
                        };
                        var client = new HttpClient(clientHandler);

                        ctx.Set("httpclient", client);

                        var eventBus = new EventBus(_store);
                        eventBus.Subscribe<RampupEvent>(new RampupEventHandler());
                        eventBus.Subscribe<RampdownEvent>(new RampupEventHandler());
                        eventBus.Subscribe<IterationLogEvent>(new IterationEventHandler());

                        var dispatcher = new Dispatcher<ICommand>();
                        dispatcher.Register<StartupThreadCommand>(new StartupThreadCommandHandler(eventBus));
                        dispatcher.Register<EndThreadCommand>(new EndThreadCommandHandler(eventBus));
                        dispatcher.Register<IterationCommand>(new IterationCommandHandler(eventBus));


                        _dispatchers.Add(dispatcher);

                        ctx.Set(nameof(IDispatcher<ICommand>), dispatcher);

                        if (test.Init != null && !string.IsNullOrEmpty(test.Init.Url))
                        {
                            var time = Stopwatch.StartNew();

                            var result = client.GetAsync(test.Init.Url).GetAwaiter().GetResult();

                            time.Stop();

                            var command = new StartupThreadCommand
                            {
                                TestId = _testId,
                                Category = "console",
                                Module = "Init",
                                Name = test.Name,
                                Message = $"Init {test.Init.Url} ended with status {result.StatusCode} after {time.ElapsedMilliseconds} ms",
                                StatusCode = result.StatusCode,
                                ElapsedMilliseconds = time.ElapsedMilliseconds,
                                IsWarmup = s.IsWarmup
                            };

                            dispatcher.SendAsync(command);
                        }


                        return ctx;
                    })
                    .OnEndPipeline(e =>
                    {
                        e.Get<HttpClient>("httpclient").Dispose();
                        var dispatcher = e.Get<IDispatcher<ICommand>>(nameof(IDispatcher<ICommand>));

                        var metric = new EndThreadCommand
                        {
                            TestId = _testId,
                            Category = "console",
                            Module = "End",
                            Name = test.Name,
                            Message = $"End Run for Thread {e.Get(ContextKeys.ThreadNumber)}",
                            Thread = e.Get<int>(ContextKeys.ThreadNumber),
                            IsWarmup = e.Settings.IsWarmup
                        };

                        dispatcher.SendAsync(metric);
                    })
                    .Task(ctx =>
                    {
                        var client = ctx.Get<HttpClient>("httpclient");

                        foreach (var url in test.Urls)
                        {
                            var time = Stopwatch.StartNew();

                            var result = client.GetAsync(url).GetAwaiter().GetResult();

                            time.Stop();

                            if (!ctx.Settings.IsWarmup)
                            {
                                //_logger.Write($"Call to {url} ended with status {result.StatusCode} after {time.ElapsedMilliseconds} ms", Opacc.Fof.Commons.Diagnostics.LogLevel.Debug, category: "console", source: test.Name, module: "LoadTest");
                            }
                        }
                    });

                if (test.Iterations > 0)
                {
                    session.SetIterations(test.Iterations);
                }

                if (test.Threads > 0)
                {
                    var rampup = test.RampupTime > 0 ? TimeSpan.FromSeconds(test.RampupTime) : TimeSpan.Zero;
                    session.SetThreads(test.Threads, rampup);
                }

                if (test.Duration > 0)
                {
                    session.SetDuration(TimeSpan.FromMinutes(test.Duration));
                }

                if (test.Interval > 0)
                {
                    session.SetInterval(TimeSpan.FromMilliseconds(test.Interval));
                }

                var result = session.RunSession();

                result.Trace(new ConsoleResultWriter());

                results.Add(new TestResult(result)
                {
                    TestCase = test.Name,
                });
            }

            return results;
        }

        public void End()
        {
            foreach (var collector in _dispatchers)
            {
                collector.Close();
            }
        }
    }
}

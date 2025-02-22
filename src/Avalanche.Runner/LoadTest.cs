using Avalanche.Runner.Logging;
using MeasureMap;
//using MeasureMap.Diagnostics;
using System.Diagnostics;
using System.Net;

namespace Avalanche.Runner
{
    public class LoadTest
    {
        private readonly Logger _logger;

        public LoadTest(Logger logger)
        {
            _logger = logger;
        }

        public IEnumerable<TestResult> Run(TestSettings settings)
        {
            var results = new List<TestResult>();

            _logger.StartTime = DateTime.Now;

            foreach (var test in settings.Tests)
            {
                var session = ProfilerSession.StartSession()
                    .AddMiddleware(new ItterationLogCollectionTaskHandler(test.Name))
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

                        var log = _logger.StartNew($"{Guid.NewGuid()}");
                        ctx.Set(nameof(ITestResultCollector), log);

                        if (test.Init != null && !string.IsNullOrEmpty(test.Init.Url))
                        {
                            var time = Stopwatch.StartNew();

                            var result = client.GetAsync(test.Init.Url).GetAwaiter().GetResult();

                            time.Stop();

                            var metric = new StartupLogEvent
                            {
                                Category = "console",
                                Module = "Init",
                                Name = test.Name,
                                Message = $"Init {test.Init.Url} ended with status {result.StatusCode} after {time.ElapsedMilliseconds} ms",
                                StatusCode = result.StatusCode,
                                ElapsedMilliseconds = time.ElapsedMilliseconds
                            };

                            log.Add(metric);
                        }


                        return ctx;
                    })
                    .OnEndPipeline(e =>
                    {
                        e.Get<HttpClient>("httpclient").Dispose();
                        var log = e.Get<ITestResultCollector>(nameof(ITestResultCollector));

                        var metric = new EndLogEvent
                        {
                            Category = "console",
                            Module = "End",
                            Name = test.Name,
                            Message = $"End Run for Thread {e.Get(ContextKeys.ThreadNumber)}",
                            Thread = e.Get<int>(ContextKeys.ThreadNumber),
                            IsWarmup = e.Settings.IsWarmup
                        };

                        log.Add(metric);
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
                    Name = test.Name,
                });
            }

            return results;
        }
    }
}

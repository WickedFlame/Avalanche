using Avalanche.WriteModel;
using Avalanche.WriteModel.Commands;
using Broadcast;
using MeasureMap;
using RestSharp;
using System.Diagnostics;
using System.Net;

namespace Avalanche.Runner
{
    public class LoadTest
    {
        private readonly string _testId;
        private readonly IDispatcher<ICommand> _dispatcher;

        public LoadTest(string testId, IDispatcher<ICommand> dispatcher)
        {
            _testId = testId;
            _dispatcher = dispatcher;
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

                        var options = new RestClientOptions()
                        {
                            FollowRedirects = true,
                            RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true,
                        };

                        var client = new RestClient(options);

                        ctx.Set("httpclient", client);
                        ctx.Set(nameof(IDispatcher<ICommand>), _dispatcher);

                        if (test.Init != null && !string.IsNullOrEmpty(test.Init.Url))
                        {
                            var time = Stopwatch.StartNew();

                            var request = new RestRequest(test.Init.Url);
                            var result = client.GetAsync(request).GetAwaiter().GetResult();

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

                            _dispatcher.SendAsync(command);
                        }

                        return ctx;
                    })
                    .OnEndPipeline(e =>
                    {
                        e.Get<RestClient>("httpclient").Dispose();

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

                        _dispatcher.SendAsync(metric);
                    })
                    .Task(ctx =>
                    {
                        var client = ctx.Get<RestClient>("httpclient");

                        foreach (var url in test.Urls)
                        {
                            try
                            {
                                var request = new RestRequest(url);
                                var result = client.GetAsync(request).GetAwaiter().GetResult();
                                if (!result.IsSuccessful)
                                {
                                    var cmd = new IterationFailedCommand
                                    {
                                        Time = DateTime.Now,
                                        TestId = _testId,
                                        TestName = test.Name,
                                        Thread = ctx.Get<int>(ContextKeys.ThreadNumber),
                                        Message = result.ErrorMessage,
                                        StatusCode = result.StatusCode
                                    };
                                    _dispatcher.SendAsync(cmd);
                                }

                                if (!ctx.Settings.IsWarmup)
                                {
                                    //_logger.Write($"Call to {url} ended with status {result.StatusCode} after {time.ElapsedMilliseconds} ms", Opacc.Fof.Commons.Diagnostics.LogLevel.Debug, category: "console", source: test.Name, module: "LoadTest");
                                }
                            }
                            catch(Exception e)
                            {
                                var cmd = new IterationFailedCommand
                                {
                                    Time = DateTime.Now,
                                    TestId = _testId,
                                    TestName = test.Name,
                                    Thread = ctx.Get<int>(ContextKeys.ThreadNumber),
                                    Message = e.Message,
                                    //StatusCode = result.StatusCode
                                };
                                _dispatcher.SendAsync(cmd);
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

                if (test.Delay > 0)
                {
                    session.AddDelay(TimeSpan.FromSeconds(test.Delay));
                }

                var result = session.RunSession();

                //
                // Wait for the console to write all results before tracing the summeray
                System.Threading.Tasks.Task.Delay(5000).Wait();

                result.Trace();

                results.Add(new TestResult(result)
                {
                    TestCase = test.Name,
                });
            }

            return results;
        }
    }
}

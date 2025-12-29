using Avalanche.Runner.Handlers;
using Avalanche.WriteModel;
using Avalanche.WriteModel.Commands;
using Broadcast;
using MeasureMap;
using Microsoft.Extensions.Logging;
using RestSharp;
using System.Diagnostics;
using System.Net;

namespace Avalanche.Runner
{
    public class TestRunner
    {
        private readonly string _testId;
        private readonly IDispatcher _dispatcher;
        private readonly ILogger<TestRunner> _logger;
        private readonly ILoggerFactory _loggerFactory;

        public TestRunner(string testId, IDispatcher dispatcher, ILoggerFactory logger)
        {
            _testId = testId;
            _dispatcher = dispatcher;
            _logger = logger.CreateLogger<TestRunner>();

            //TODO: remove after Authorization Handlers are injected properly
            _loggerFactory = logger;
        }

        public IEnumerable<TestResult> Run(Scenario settings)
        {
            var results = new List<TestResult>();

            foreach (var test in settings.TestCases)
            {
                var requests = test.Urls.Select(u => new RequestUrl(u));

                var session = ProfilerSession.StartSession()
                    .AddMiddleware(new ItterationLogCollectionTaskHandler(test.Name, _testId))
                    .OnStartPipeline(s =>
                    {
                        var ctx = new MeasureMap.ExecutionContext(s);

                        var options = new RestSharp.RestClientOptions()
                        {
                            FollowRedirects = true,
                            RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true,
                        };

                        if (test.UseCookies)
                        {
                            options.CookieContainer = new CookieContainer();
                        }

                        var client = new RestClient(options);

                        ctx.Set("httpclient", client);
                        ctx.Set(nameof(IDispatcher), _dispatcher);

                        var url = test.Init != null && !string.IsNullOrEmpty(test.Init.Url) ? test.Init.Url : test.Urls.FirstOrDefault();

                        if (settings.Authorization != null && !string.IsNullOrEmpty(settings.Authorization.Type))
                        {
                            //TODO: Inject all handlers from outside the TestRunner
                            IExecutionHandler oauth = settings.Authorization.Type.ToLower() switch
                            {
                                "oauth" => new Handlers.OAuthAuthenticationHandler(_loggerFactory),
                                "none" => new Handlers.DefaultAuthenticationHandler(),
                                _ => new Handlers.DefaultAuthenticationHandler()
                            };

                            oauth.Execute(ctx, settings);
                        }
                        

                        if (!string.IsNullOrEmpty(url))
                        {
                            try
                            {
                                var time = Stopwatch.StartNew();

                                var execution = new RequestExecution(client);
                                var result = execution.Execute(new RequestUrl(url));

                                time.Stop();

                                var command = new StartupThreadCommand
                                {
                                    TestId = _testId,
                                    Category = "console",
                                    Module = "Init",
                                    TestCase = test.Name,
                                    Message = $"Init {url} ended with status {result.StatusCode} after {time.ElapsedMilliseconds} ms",
                                    StatusCode = result.StatusCode,
                                    ElapsedMilliseconds = time.ElapsedMilliseconds,
                                    IsWarmup = s.IsWarmup
                                };

                                _dispatcher.Enqueue(command);
                            }
                            catch (Exception e)
                            {
                                _logger.LogError(e, "Error in OnStartPipeline Event for Test {TestId}", _testId);
                            }
                        }

                        return ctx;
                    })
                    .OnEndPipeline(e =>
                    {
                        e.Get<IRestClient>("httpclient").Dispose();

                        var metric = new EndThreadCommand
                        {
                            TestId = _testId,
                            TestCase = test.Name,
                            Message = $"End Run for Thread {e.Get(ContextKeys.ThreadId)}",
                            ThreadId = e.Get<int>(ContextKeys.ThreadId),
                            IsWarmup = e.Settings.IsWarmup
                        };

                        _dispatcher.Enqueue(metric);
                    })
                    .PreExecute(ctx =>
                    {
                        ctx.Set("ContentLength", 0L);
                    })
                    .Task(ctx =>
                    {
                        var client = ctx.Get<IRestClient>("httpclient");
                        var execution = new RequestExecution(client);

                        foreach (var req in requests)
                        {
                            try
                            {
                                var result = execution.Execute(req);
                                if (!result.IsSuccessful)
                                {
                                    var cmd = new IterationFailedCommand
                                    {
                                        Time = DateTime.Now,
                                        TestId = _testId,
                                        TestCase = test.Name,
                                        ThreadId = ctx.Get<int>(ContextKeys.ThreadId),
                                        Message = result.ErrorMessage,
                                        StatusCode = result.StatusCode,
                                        IsWarmup = ctx.Settings.IsWarmup
                                    };
                                    _dispatcher.Enqueue(cmd);

                                    _logger.LogInformation("Call to {Url} for Test {TestId} resulted in StatusCode {StatusCode}", req.Url, _testId, result.StatusCode);
                                }

                                ctx.Set("ContentLength", result.ContentLength);
                            }
                            catch(Exception e)
                            {
                                var cmd = new IterationFailedCommand
                                {
                                    Time = DateTime.Now,
                                    TestId = _testId,
                                    TestCase = test.Name,
                                    ThreadId = ctx.Get<int>(ContextKeys.ThreadId),
                                    Message = e.Message,
                                    IsWarmup = ctx.Settings.IsWarmup
                                    //StatusCode = result.StatusCode
                                };
                                _dispatcher.Enqueue(cmd);

                                _logger.LogError(e, "Call to {Url} for Test {TestId} caused an error", req.Url, _testId);
                            }
                        }
                    });

                if (test.Iterations > 0)
                {
                    session.SetIterations(test.Iterations);
                }

                if (test.Users > 0)
                {
                    var rampup = test.RampupTime > 0 ? TimeSpan.FromSeconds(test.RampupTime) : TimeSpan.Zero;
                    session.SetThreads(test.Users, rampup);
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

                session.SetMinLogLevel(MeasureMap.Diagnostics.LogLevel.Warning);

                var result = session.RunSession();

                //
                // Wait for the console to write all results before tracing the summeray
                System.Threading.Tasks.Task.Delay(5000).Wait();
                
                //result.Trace();

                results.Add(new TestResult(result)
                {
                    TestCase = test.Name,
                });
            }

            return results;
        }
    }
}

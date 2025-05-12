using Avalanche.WriteModel.Events;
using Broadcast;
using Microsoft.AspNetCore.Mvc;

namespace Avalanche.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IEventBus _eventBus;
        public EventController(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        [HttpPost]
        [Route("starttest")]
        public IActionResult StartTest([FromBody] StartTestEvent evnt)
        {
            _eventBus.Publish(Guid.NewGuid().ToString(), evnt.StartTime, evnt);

            return Ok();
        }

        [HttpPost]
        [Route("endtest")]
        public IActionResult EndTest([FromBody] EndTestEvent evnt)
        {
            _eventBus.Publish(Guid.NewGuid().ToString(), evnt.EndTime, evnt);

            return Ok();
        }

        [HttpPost]
        [Route("threadsummary")]
        public IActionResult ThreadSummary([FromBody] ThreadSummaryEvent evnt)
        {
            _eventBus.Publish(Guid.NewGuid().ToString(), DateTime.Now, evnt);

            return Ok();
        }

        [HttpPost]
        [Route("testsummary")]
        public IActionResult TestSummary([FromBody] TestSummaryEvent evnt)
        {
            _eventBus.Publish(Guid.NewGuid().ToString(), DateTime.Now, evnt);

            return Ok();
        }

        [HttpPost]
        [Route("iteration")]
        public IActionResult IterationSummary([FromBody] IterationLogEvent evnt)
        {
            _eventBus.Publish(Guid.NewGuid().ToString(), evnt.Time, evnt);

            return Ok();
        }

        [HttpPost]
        [Route("rampup")]
        public IActionResult RampupSummary([FromBody] RampupEvent evnt)
        {
            _eventBus.Publish(Guid.NewGuid().ToString(), evnt.Time, evnt);

            return Ok();
        }

        [HttpPost]
        [Route("rampdown")]
        public IActionResult RampdonwSummary([FromBody] RampdownEvent evnt)
        {
            _eventBus.Publish(Guid.NewGuid().ToString(), evnt.Time, evnt);

            return Ok();
        }
    }
}

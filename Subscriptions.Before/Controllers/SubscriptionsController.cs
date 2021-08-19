using System.Threading.Tasks;
using AzureServiceBus.EventBus;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Subscriptions.Before.Commands;

namespace Subscriptions.Before.Controllers
{
    [ApiController]
    //[Route("[controller]")]
    public class SubscriptionsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IEventBus _eventBus;
        public SubscriptionsController(IMediator mediator,
            IEventBus eventBus)
        {
            _mediator = mediator; _eventBus = eventBus;
        }

        [HttpPost("Subscribe")]
        public async Task<IActionResult> Subscribe([FromBody] SubscribeRequest request)
        {
            await _mediator.Send(request); 

            return Ok();
        }
    }
}
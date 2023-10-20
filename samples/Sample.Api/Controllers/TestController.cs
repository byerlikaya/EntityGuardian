using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sample.Api.Handler;

namespace Sample.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        private IMediator _mediator;

        protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();


        [HttpGet("insert")]

        public async Task<IActionResult> Insert([FromQuery] TestInsert request)
        {
            await Mediator.Send(request);

            return Ok();
        }

        [HttpGet("update")]

        public async Task<IActionResult> Update([FromQuery] TestUpdate request)
        {
            await Mediator.Send(request);

            return Ok();
        }

        [HttpGet("delete")]

        public async Task<IActionResult> Delete([FromQuery] TestDelete request)
        {
            await Mediator.Send(request);

            return Ok();
        }
    }
}
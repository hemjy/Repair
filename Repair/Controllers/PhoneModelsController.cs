using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repair.Application.Common;
using Repair.Application.Features.PhoneModels.Commands;
using Repair.Application.Features.PhoneModels.Queries;

namespace Repair.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhoneModelsController : BaseApiController
    {
        [ProducesResponseType(200, Type = typeof(Result<Guid>))]
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreatePhoneModelCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
        [ProducesResponseType(200, Type = typeof(Result<Guid>))]
        [HttpPut]
        public async Task<IActionResult> Update([FromForm] UpdatePhoneModelCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
        [ProducesResponseType(200, Type = typeof(Result<Guid>))]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetPhoneModelsQuery query)
        {
            return Ok(await Mediator.Send(query));
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repair.Application.Common;
using Repair.Application.Features.PhoneParts.Commands;
using Repair.Application.Features.PhoneParts.Queries;

namespace Repair.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhonePartsController : BaseApiController
    {
        [ProducesResponseType(200, Type = typeof(Result<Guid>))]
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreatePhonePartCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
        [ProducesResponseType(200, Type = typeof(Result<Guid>))]
        [HttpPut]
        public async Task<IActionResult> Update([FromForm] UpdatePhonePartCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
        [ProducesResponseType(200, Type = typeof(Result<Guid>))]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetPhonePartsQuery query)
        {
            return Ok(await Mediator.Send(query));
        }
    }
}

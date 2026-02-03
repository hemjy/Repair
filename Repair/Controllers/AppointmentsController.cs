using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repair.Application.Common;
using Repair.Application.Features.Appointments.Commands;
using Repair.Application.Features.Appointments.Queries;

namespace Repair.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController : BaseApiController
    {
        [ProducesResponseType(200, Type = typeof(Result<Guid>))]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAppointmentCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
        [ProducesResponseType(200, Type = typeof(Result<Guid>))]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateAppointmentCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
        [ProducesResponseType(200, Type = typeof(Result<Guid>))]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetAppointmentsQuery query)
        {
            return Ok(await Mediator.Send(query));
        }
    }
}

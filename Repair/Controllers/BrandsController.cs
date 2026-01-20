using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repair.Application.Common;
using Repair.Application.Features.Brands.Commands;
using Repair.Application.Features.Brands.Queries;

namespace Repair.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandsController : BaseApiController
    {
        [ProducesResponseType(200, Type = typeof(Result<Guid>))]
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateBrandCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
        [ProducesResponseType(200, Type = typeof(Result<Guid>))]
        [HttpPut]
        public async Task<IActionResult> Update([FromForm] UpdateBrandCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
        [ProducesResponseType(200, Type = typeof(Result<Guid>))]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetBrandsQuery query)
        {
            return Ok(await Mediator.Send(query));
        }
    }
}

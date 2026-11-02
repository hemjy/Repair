using Microsoft.AspNetCore.Mvc;
using Repair.Application.Common;
using Repair.Application.Features.Appointments.Commands;
using Repair.Application.Features.BlogPosts.Commands;
using Repair.Application.Features.PhoneParts.Queries;

namespace Repair.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogsController : BaseApiController
    {
        [ProducesResponseType(200, Type = typeof(Result<Guid>))]
        [HttpPost("categories")]
        public async Task<IActionResult> CreateCategory([FromBody] CreateBlogCategoryCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpPut("categories")]
        public async Task<IActionResult> UpdateCategory([FromBody] UpdateBlogCategoryCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
        [HttpDelete("categories")]
        public async Task<IActionResult> DeleteCategory([FromBody] DeleteBlogCategoryCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [ProducesResponseType(200, Type = typeof(Result<Guid>))]
        [HttpGet("Categories")]
        public async Task<IActionResult> GetCategories([FromQuery] GetBlogCategoriesQuery query)
        {
            return Ok(await Mediator.Send(query));
        }
        [ProducesResponseType(200, Type = typeof(Result<Guid>))]
        [HttpPost("posts")]
        public async Task<IActionResult> CreatePost([FromForm] CreateBlogPostCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpPut("posts")]
        public async Task<IActionResult> UpdatePost([FromForm] UpdateBlogPostCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
        [HttpPut("posts/status")]
        public async Task<IActionResult> UpdatePostStatus([FromBody] UpdateBlogPostStatusCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
        [HttpPut("posts/feature")]
        public async Task<IActionResult> MarkAsFeature([FromBody] MarkPostAsFeaturedCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
        [HttpDelete("posts")]
        public async Task<IActionResult> DeletePost([FromBody] UpdateBlogPostCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [ProducesResponseType(200, Type = typeof(Result<Guid>))]
        [HttpGet("posts")]
        public async Task<IActionResult> Getposts([FromQuery] GetBlogPostsQuery query)
        {
            return Ok(await Mediator.Send(query));
        }
    }
}

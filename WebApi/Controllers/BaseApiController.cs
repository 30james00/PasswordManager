using MediatR;
using Microsoft.AspNetCore.Mvc;
using PasswordManager.Application.Core;

namespace PasswordManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseApiController : ControllerBase
    {
        protected IMediator _mediator;

        public BaseApiController(IMediator mediator)
        {
            _mediator = mediator;
        }

        protected ActionResult HandleResult<T>(ApiResult<T>? result)
        {
            if (result == null) return NotFound();
            if (result.IsForbidden) return new ObjectResult(result.Error) { StatusCode = 403};
            if (result.IsSuccess && result.Value != null) return Ok(result.Value);
            if (result.IsSuccess && result.Value == null) return NotFound();
            return BadRequest(result.Error);
        }

        protected ActionResult HandlePagedResult<T>(ApiResult<T>? result)
        {
            if (result == null) return NotFound();
            if (result.IsForbidden) return Forbid();
            if (result.IsSuccess && result.Value != null) return Ok(result.Value);
            if (result.IsSuccess && result.Value == null) return NotFound();
            return BadRequest(result.Error);
        }
    }
}
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PasswordManager.Application.Core;

namespace PasswordManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseApiController : ControllerBase
    {
        private IMediator _mediator;

        protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();

        protected ActionResult HandleResult<T>(ApiResult<T>? result)
        {
            if (result == null) return NotFound();
            if (result.IsForbidden) return Forbid();
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
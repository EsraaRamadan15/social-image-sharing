using Application.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/users")]
public sealed class UsersController : ControllerBase
{
    [Authorize]
    [HttpGet("me")]
    public IActionResult Me([FromServices] ICurrentUserService currentUserService)
    {
        return Ok(new
        {
            currentUserService.UserId,
            currentUserService.UserName,
            currentUserService.IsAuthenticated
        });
    }
}

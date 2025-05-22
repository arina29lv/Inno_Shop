using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserControl.Application.Commands.UserCommands;
using UserControl.Application.Queries.UserQueries;

namespace UserControl.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _mediator.Send(new GetAllUsersQuery());
        return Ok(users);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUserById([FromRoute]int id)
    {
        var user = await _mediator.Send(new GetUserByIdQuery(id));
        if (user == null)
            NotFound();
        
        return Ok(user);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateUser([FromRoute] int id, [FromBody] UpdateUserCommand command)
    {
        if(id != command.Id)
            return BadRequest("ID do not match.");
        
        var success = await _mediator.Send(command);
        
        if(!success)
            return NotFound();
        
        return Ok(new {Message = "User updated."});
    }
    
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteUser([FromRoute] int id)
    {
        var success = await _mediator.Send(new DeleteUserCommand { Id = id });
        
        if (!success) 
            return NotFound();
        return NoContent();
    }

    [HttpPost("{id}/deactivate")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeactivateUser([FromRoute] int id)
    {
        var result = await _mediator.Send(new DeactivateUserCommand { UserId = id });
        return result ? NoContent() : NotFound();
    }

    [HttpPost("{id}/activate")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ActivateUser([FromRoute] int id)
    {
        var result = await _mediator.Send(new ActivateUserCommand { UserId = id });
        return result ? NoContent() : NotFound();
    }
}

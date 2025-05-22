using MediatR;
using Microsoft.AspNetCore.Mvc;
using UserControl.Application.Commands.AuthCommands;
using UserControl.Application.DTOs.AuthDTOs;

namespace UserControl.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterUserCommand command)
    {
        var result = await _mediator.Send(command);
        return result.isRegistered ?Ok(new {NewId = result.id, Message = "User registered successfully; Confirm you email."}) : BadRequest(new {error = $"User with such email already exists with id {result.id}"});
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginUser([FromBody] LoginUserCommand command)
    {
        var result = await _mediator.Send(command);
        if (result is null)
            return Unauthorized();
        
        return Ok(new { result.AccessToken, result.RefreshToken });
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command)
    {
        var result = await _mediator.Send(command);
        if (result is null)
            return Unauthorized();
        
        return Ok(new {result.AccessToken, result.RefreshToken});
    }
    
    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string email, [FromQuery] string token)
    {
        var result = await _mediator.Send(new ConfirmEmailCommand {Email = email, Token = token});

        if (!result)
            return BadRequest("Invalid confirmation link");

        return Ok("Email confirmed successfully!");
    }
    
    [HttpPost("request-password-reset")]
    public async Task<IActionResult> RequestPasswordReset([FromBody] RequestPasswordResetCommand command)
    {
        var result = await _mediator.Send(command);
        return result ? Ok("Reset link was successfully sent to your email!") : BadRequest("User not found or email not confirmed.");
    }
    
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromQuery] string token, [FromQuery] string email, [FromBody] ResetPasswordDto dto)
    {
        var command = new ResetPasswordCommand
        {
            Email = email,
            Token = token,
            NewPassword = dto.NewPassword,
        };
    
        var result = await _mediator.Send(command);
        return result ? Ok("Password was successfully updated!") : BadRequest("Invalid or expired token.");
    }
}
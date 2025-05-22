using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductControl.Application.Command;
using ProductControl.Application.Queries;

namespace ProductControl.Presentation.Controllers;


[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userId is null)
            return Unauthorized();

        command.UserId = int.Parse(userId.Value);
        var id = await _mediator.Send(command);
        return Ok(id);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAllProducts(
        [FromQuery] bool? isAvailable,
        [FromQuery] decimal? priceMin,
        [FromQuery] decimal? priceMax,
        [FromQuery] string? nameContains)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier);
        var userRole = User.FindFirst(ClaimTypes.Role);
        var query = new GetAllProductsQuery()
        {
            UserId = int.Parse(userId!.Value),
            UserRole = userRole!.Value,
            IsAvailable = isAvailable,
            PriceMin = priceMin,
            PriceMax = priceMax,
            NameContains = nameContains
        };

        var products = await _mediator.Send(query);
        return Ok(products);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetProductById([FromRoute] int id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier);
        var role = User.FindFirst(ClaimTypes.Role);

        var query = new GetProductByIdQuery(id)
        {
            UserId = int.Parse(userId!.Value),
            UserRole = role!.Value
        };
        var product = await _mediator.Send(query);
        if (product == null)
            return NotFound();

        return Ok(product);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateProduct([FromRoute] int id, [FromBody] UpdateProductCommand command)
    {
        var updatorId = User.FindFirst(ClaimTypes.NameIdentifier);
        var updatorRole = User.FindFirst(ClaimTypes.Role);
      
        if(id != command.Id)
            return BadRequest("ID do not match.");
        
        command.UpdatorId = int.Parse(updatorId!.Value);
        command.UpdatorRole = updatorRole!.Value;
        var success = await _mediator.Send(command);
        
        if(!success)
            return NotFound();
        
        return Ok(new {Message = "Product updated."});
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteProduct([FromRoute] int id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier);
        var userRole = User.FindFirst(ClaimTypes.Role);

        var command = new DeleteProductCommand
        {
            Id = id,
            UserId = int.Parse(userId!.Value),
            UserRole = userRole!.Value
        };
        
        var success = await _mediator.Send(command);
        
        if(!success)
            return NotFound();

        return NoContent();
    }

    
    [HttpGet("user/{userId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetProductsByUserId([FromRoute] int userId)
    {
        var products = await _mediator.Send(new GetProductsByUserIdQuery(userId));
        return Ok(products);
    }
}
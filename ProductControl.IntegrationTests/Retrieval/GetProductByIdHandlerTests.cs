using AutoMapper;
using ProductControl.Application.DTOs;
using ProductControl.Application.Handlers;
using ProductControl.Application.Queries;
using ProductControl.Domain.Models;
using ProductControl.Infrastructure.Persistence;
using ProductControl.Infrastructure.Repositories;
using ProductControl.IntegrationTests.Base;

namespace ProductControl.IntegrationTests.Retrieval;

public class GetProductByIdHandlerTests : IntegrationTestsBase
{
    private readonly IMapper _mapper;
    private GetProductByIdHandler _handler;

    public GetProductByIdHandlerTests()
    {
        _mapper = new Mapper(new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Product, ProductDto>();
        }));
    }

    private void InitHandler()
    {
        var repo = new ProductRepository(new ProductDbContext(ProductDbOptions));
        _handler = new GetProductByIdHandler(repo, _mapper);
    }

    [Fact]
    public async Task Handle_ShouldReturnProduct_WhenCalledByAdmin()
    {
        /*arrange*/
        var product = new Product
        {
            Name = "Admin Access Product",
            Description = "Accessible by admin",
            Price = 100,
            IsAvailable = true,
            UserId = 5
        };
        await SeedProductAsync(product);
        InitHandler();

        var query = new GetProductByIdQuery(product.Id)
        {
            UserId = 999,
            UserRole = "Admin"
        };

        /*act*/
        var result = await _handler.Handle(query, CancellationToken.None);

        /*assert*/
        Assert.NotNull(result);
        Assert.Equal(product.Name, result!.Name);
    }

    [Fact]
    public async Task Handle_ShouldReturnProduct_WhenCalledByOwner()
    {
        /*arrange*/
        var product = new Product
        {
            Name = "Owner's Product",
            Description = "Owned by user",
            Price = 50,
            IsAvailable = true,
            UserId = 42
        };
        await SeedProductAsync(product);
        InitHandler();

        var query = new GetProductByIdQuery(product.Id)
        {
            UserId = 42,
            UserRole = "User"
        };

        /*act*/
        var result = await _handler.Handle(query, CancellationToken.None);

        /*assert*/
        Assert.NotNull(result);
        Assert.Equal(product.Name, result!.Name);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenUserIsNotOwnerAndNotAdmin()
    {
        /*arrange*/
        var product = new Product
        {
            Name = "Forbidden Product",
            Description = "User not allowed",
            Price = 30,
            IsAvailable = true,
            UserId = 1
        };
        await SeedProductAsync(product);
        InitHandler();

        var query = new GetProductByIdQuery(product.Id)
        {
            UserId = 2,
            UserRole = "User"
        };

        /*act*/
        var result = await _handler.Handle(query, CancellationToken.None);

        /*assert*/
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenProductNotFound()
    {
        /*arrange*/
        InitHandler();

        var query = new GetProductByIdQuery(999)
        {
            UserId = 1,
            UserRole = "Admin"
        };

        /*act*/
        var result = await _handler.Handle(query, CancellationToken.None);

        /*assert*/
        Assert.Null(result);
    }
}
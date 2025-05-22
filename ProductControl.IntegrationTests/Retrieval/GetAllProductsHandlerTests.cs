using AutoMapper;
using ProductControl.Application.DTOs;
using ProductControl.Application.Handlers;
using ProductControl.Application.Queries;
using ProductControl.Domain.Models;
using ProductControl.Infrastructure.Persistence;
using ProductControl.Infrastructure.Repositories;
using ProductControl.IntegrationTests.Base;

namespace ProductControl.IntegrationTests.Retrieval;

public class GetAllProductsHandlerTests : IntegrationTestsBase
{
    private readonly IMapper _mapper;

    public GetAllProductsHandlerTests()
    {
        _mapper = new Mapper(new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Product, ProductDto>();
        }));
    }

    private GetAllProductsHandler CreateHandler()
    {
        var repo = new ProductRepository(new ProductDbContext(ProductDbOptions));
        return new GetAllProductsHandler(repo, _mapper);
    }

    [Fact]
    public async Task Handle_ShouldReturnAllProducts_WhenUserIsAdmin()
    {
        await SeedProductAsync(new Product { Name = "A", Description = "Admin Product A", Price = 10, IsAvailable = true, UserId = 1 });
        await SeedProductAsync(new Product { Name = "B", Description = "Admin Product B", Price = 20, IsAvailable = false, UserId = 2 });

        var handler = CreateHandler();

        var result = await handler.Handle(new GetAllProductsQuery
        {
            UserRole = "Admin",
            UserId = 999
        }, CancellationToken.None);

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task Handle_ShouldReturnOnlyUserProducts_WhenUserIsNotAdmin()
    {
        await SeedProductAsync(new Product { Name = "Owned", Description = "User Product", Price = 15, IsAvailable = true, UserId = 42 });
        await SeedProductAsync(new Product { Name = "Other", Description = "Other User's Product", Price = 25, IsAvailable = true, UserId = 43 });

        var handler = CreateHandler();

        var result = await handler.Handle(new GetAllProductsQuery
        {
            UserRole = "User",
            UserId = 42
        }, CancellationToken.None);

        Assert.Single(result);
        Assert.Equal("Owned", result.First().Name);
    }

    [Fact]
    public async Task Handle_ShouldFilterByAvailability()
    {
        await SeedProductAsync(new Product { Name = "Available", Description = "Desc", Price = 30, IsAvailable = true, UserId = 1 });
        await SeedProductAsync(new Product { Name = "Unavailable", Description = "Desc", Price = 30, IsAvailable = false, UserId = 1 });

        var handler = CreateHandler();

        var result = await handler.Handle(new GetAllProductsQuery
        {
            UserRole = "Admin",
            IsAvailable = true
        }, CancellationToken.None);

        Assert.Single(result);
        Assert.Equal("Available", result.First().Name);
    }

    [Fact]
    public async Task Handle_ShouldFilterByPriceRange()
    {
        await SeedProductAsync(new Product { Name = "Cheap", Description = "Low price", Price = 5, IsAvailable = true, UserId = 1 });
        await SeedProductAsync(new Product { Name = "Moderate", Description = "Mid price", Price = 15, IsAvailable = true, UserId = 1 });
        await SeedProductAsync(new Product { Name = "Expensive", Description = "High price", Price = 50, IsAvailable = true, UserId = 1 });

        var handler = CreateHandler();

        var result = await handler.Handle(new GetAllProductsQuery
        {
            UserRole = "Admin",
            PriceMin = 10,
            PriceMax = 30
        }, CancellationToken.None);

        Assert.Single(result);
        Assert.Equal("Moderate", result.First().Name);
    }

    [Fact]
    public async Task Handle_ShouldFilterByNameContains()
    {
        await SeedProductAsync(new Product { Name = "Super Watch", Description = "Desc", Price = 100, IsAvailable = true, UserId = 1 });
        await SeedProductAsync(new Product { Name = "Laptop", Description = "Desc", Price = 1000, IsAvailable = true, UserId = 1 });

        var handler = CreateHandler();

        var result = await handler.Handle(new GetAllProductsQuery
        {
            UserRole = "Admin",
            NameContains = "watch"
        }, CancellationToken.None);

        Assert.Single(result);
        Assert.Equal("Super Watch", result.First().Name);
    }

    [Fact]
    public async Task Handle_ShouldApplyAllFiltersTogether()
    {
        await SeedProductAsync(new Product
        {
            Name = "Gaming Laptop",
            Description = "Powerful machine",
            Price = 1200,
            IsAvailable = true,
            UserId = 42
        });

        await SeedProductAsync(new Product
        {
            Name = "Old Laptop",
            Description = "Outdated model",
            Price = 800,
            IsAvailable = false,
            UserId = 42
        });

        var handler = CreateHandler();

        var result = await handler.Handle(new GetAllProductsQuery
        {
            UserRole = "User",
            UserId = 42,
            NameContains = "Laptop",
            IsAvailable = true,
            PriceMin = 1000
        }, CancellationToken.None);

        Assert.Single(result);
        Assert.Equal("Gaming Laptop", result.First().Name);
    }
}
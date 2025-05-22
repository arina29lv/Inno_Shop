using AutoMapper;
using ProductControl.Application.DTOs;
using ProductControl.Application.Handlers;
using ProductControl.Application.Queries;
using ProductControl.Domain.Models;
using ProductControl.Infrastructure.Persistence;
using ProductControl.Infrastructure.Repositories;
using ProductControl.IntegrationTests.Base;

namespace ProductControl.IntegrationTests.Retrieval;

public class GetProductsByUserIdHandlerTests : IntegrationTestsBase
{
    private readonly IMapper _mapper;

    public GetProductsByUserIdHandlerTests()
    {
        _mapper = new Mapper(new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Product, ProductDto>();
        }));
    }

    private GetProductsByUserIdHandler CreateHandler()
    {
        var repo = new ProductRepository(new ProductDbContext(ProductDbOptions));
        return new GetProductsByUserIdHandler(repo, _mapper);
    }

    [Fact]
    public async Task Handle_ShouldReturnAllProductsForGivenUser()
    {
        /*arrange*/
        var userId = 123;

        var products = new[]
        {
            new Product
            {
                Name = "UserProduct1",
                Description = "desc 1",
                Price = 10,
                IsAvailable = true,
                UserId = userId
            },
            new Product
            {
                Name = "UserProduct2",
                Description = "desc 2",
                Price = 20,
                IsAvailable = true,
                UserId = userId
            },
            new Product
            {
                Name = "OtherUserProduct",
                Description = "should not appear",
                Price = 99,
                IsAvailable = true,
                UserId = 999
            }
        };

        foreach (var product in products)
        {
            await SeedProductAsync(product);
        }

        var handler = CreateHandler();

        /*act*/
        var result = await handler.Handle(new GetProductsByUserIdQuery(userId), CancellationToken.None);

        /*assert*/
        Assert.Equal(2, result.Count());
        Assert.All(result, p => Assert.Equal(userId, p.UserId));
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenUserHasNoProducts()
    {
        /*arrange*/
        var userId = 777;
        var handler = CreateHandler();

        /*act*/
        var result = await handler.Handle(new GetProductsByUserIdQuery(userId), CancellationToken.None);

        /*assert*/
        Assert.Empty(result);
    }
}
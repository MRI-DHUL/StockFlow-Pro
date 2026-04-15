using FluentAssertions;
using MapsterMapper;
using Moq;
using StockFlow.Application.DTOs;
using StockFlow.Application.Interfaces;
using StockFlow.Application.Products.Queries;
using StockFlow.Domain.Entities;
using Xunit;

namespace StockFlow.UnitTests.Application.Products;

public class GetAllProductsQueryHandlerTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ICacheService> _cacheServiceMock;
    private readonly GetAllProductsQueryHandler _handler;

    public GetAllProductsQueryHandlerTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _mapperMock = new Mock<IMapper>();
        _cacheServiceMock = new Mock<ICacheService>();
        _handler = new GetAllProductsQueryHandler(
            _productRepositoryMock.Object,
            _mapperMock.Object,
            _cacheServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsAllProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Product 1",
                SKU = "SKU-001",
                UnitPrice = 10.99m,
                CreatedAt = DateTime.UtcNow
            },
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Product 2",
                SKU = "SKU-002",
                UnitPrice = 20.99m,
                CreatedAt = DateTime.UtcNow
            }
        };

        var productDtos = new List<ProductDto>
        {
            new ProductDto { Id = products[0].Id, Name = "Product 1", SKU = "SKU-001", UnitPrice = 10.99m },
            new ProductDto { Id = products[1].Id, Name = "Product 2", SKU = "SKU-002", UnitPrice = 20.99m }
        };

        _cacheServiceMock
            .Setup(x => x.GetAsync<List<ProductDto>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((List<ProductDto>?)null);

        _productRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        _mapperMock
            .Setup(x => x.Map<List<ProductDto>>(It.IsAny<List<Product>>()))
            .Returns(productDtos);

        var query = new GetAllProductsQuery();

        // Act
        var result = await _handler.Handle(query, default);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.First().Name.Should().Be("Product 1");
        result.Last().Name.Should().Be("Product 2");

        _productRepositoryMock.Verify(x => x.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NoProducts_ReturnsEmptyList()
    {
        // Arrange
        _cacheServiceMock
            .Setup(x => x.GetAsync<List<ProductDto>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((List<ProductDto>?)null);

        _productRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Product>());

        _mapperMock
            .Setup(x => x.Map<List<ProductDto>>(It.IsAny<List<Product>>()))
            .Returns(new List<ProductDto>());

        var query = new GetAllProductsQuery();

        // Act
        var result = await _handler.Handle(query, default);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }
}

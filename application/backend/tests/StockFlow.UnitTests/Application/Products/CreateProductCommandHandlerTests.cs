using FluentAssertions;
using MapsterMapper;
using Moq;
using StockFlow.Application.DTOs;
using StockFlow.Application.Interfaces;
using StockFlow.Application.Products.Commands;
using StockFlow.Domain.Entities;
using Xunit;

namespace StockFlow.UnitTests.Application.Products;

public class CreateProductCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ICacheService> _cacheServiceMock;
    private readonly CreateProductCommandHandler _handler;

    public CreateProductCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _cacheServiceMock = new Mock<ICacheService>();
        _handler = new CreateProductCommandHandler(
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            _cacheServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidProduct_ReturnsProductDto()
    {
        // Arrange
        var createDto = new CreateProductDto
        {
            Name = "Test Product",
            SKU = "TEST-001",
            Category = "Electronics",
            UnitPrice = 99.99m,
            Description = "Test Description"
        };

        var command = new CreateProductCommand(createDto);

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = createDto.Name,
            SKU = createDto.SKU,
            Category = createDto.Category,
            UnitPrice = createDto.UnitPrice,
            Description = createDto.Description,
            CreatedAt = DateTime.UtcNow
        };

        var productDto = new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            SKU = product.SKU,
            Category = product.Category,
            UnitPrice = product.UnitPrice,
            Description = product.Description
        };

        _mapperMock
            .Setup(x => x.Map<Product>(createDto))
            .Returns(product);

        _unitOfWorkMock
            .Setup(x => x.Products.AddAsync(It.IsAny<Product>(), default))
            .ReturnsAsync(product);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(default))
            .ReturnsAsync(1);

        _mapperMock
            .Setup(x => x.Map<ProductDto>(It.IsAny<Product>()))
            .Returns(productDto);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(createDto.Name);
        result.SKU.Should().Be(createDto.SKU);
        result.UnitPrice.Should().Be(createDto.UnitPrice);

        _unitOfWorkMock.Verify(x => x.Products.AddAsync(It.IsAny<Product>(), default), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
    }
}

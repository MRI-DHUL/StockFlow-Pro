using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MapsterMapper;
using MockQueryable.Moq;
using Moq;
using StockFlow.Application.DTOs;
using StockFlow.Application.Interfaces;
using StockFlow.Application.Services;
using StockFlow.Domain.Entities;
using Xunit;

namespace StockFlow.UnitTests.Application.Services;

public class ProductServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ICacheService> _cacheServiceMock;
    private readonly Mock<IValidator<CreateProductDto>> _createValidatorMock;
    private readonly Mock<IValidator<UpdateProductDto>> _updateValidatorMock;
    private readonly ProductService _productService;

    public ProductServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _productRepositoryMock = new Mock<IProductRepository>();
        _mapperMock = new Mock<IMapper>();
        _cacheServiceMock = new Mock<ICacheService>();
        _createValidatorMock = new Mock<IValidator<CreateProductDto>>();
        _updateValidatorMock = new Mock<IValidator<UpdateProductDto>>();

        _unitOfWorkMock.Setup(u => u.Products).Returns(_productRepositoryMock.Object);

        _productService = new ProductService(
            _unitOfWorkMock.Object,
            _productRepositoryMock.Object,
            _mapperMock.Object,
            _cacheServiceMock.Object,
            _createValidatorMock.Object,
            _updateValidatorMock.Object
        );
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsProductDto()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = new Product
        {
            Id = productId,
            Name = "Test Product",
            SKU = "TEST-001",
            UnitPrice = 99.99m
        };
        var productDto = new ProductDto
        {
            Id = productId,
            Name = "Test Product",
            SKU = "TEST-001",
            UnitPrice = 99.99m
        };

        _productRepositoryMock
            .Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        _mapperMock
            .Setup(m => m.Map<ProductDto>(product))
            .Returns(productDto);

        // Act
        var result = await _productService.GetByIdAsync(productId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(productId);
        result.Name.Should().Be("Test Product");
        result.SKU.Should().Be("TEST-001");
        result.UnitPrice.Should().Be(99.99m);

        _productRepositoryMock.Verify(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
        _mapperMock.Verify(m => m.Map<ProductDto>(product), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        var productId = Guid.NewGuid();

        _productRepositoryMock
            .Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _productService.GetByIdAsync(productId);

        // Assert
        result.Should().BeNull();

        _productRepositoryMock.Verify(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
        _mapperMock.Verify(m => m.Map<ProductDto>(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public async Task GetAllAsync_WithCachedData_ReturnsCachedProducts()
    {
        // Arrange
        var cachedProducts = new List<ProductDto>
        {
            new() { Id = Guid.NewGuid(), Name = "Product 1", SKU = "SKU-001", UnitPrice = 10.00m },
            new() { Id = Guid.NewGuid(), Name = "Product 2", SKU = "SKU-002", UnitPrice = 20.00m }
        };

        _cacheServiceMock
            .Setup(c => c.GetAsync<List<ProductDto>>("products_all", It.IsAny<CancellationToken>()))
            .ReturnsAsync(cachedProducts);

        // Act
        var result = await _productService.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(cachedProducts);

        _cacheServiceMock.Verify(c => c.GetAsync<List<ProductDto>>("products_all", It.IsAny<CancellationToken>()), Times.Once);
        _productRepositoryMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetAllAsync_WithoutCachedData_RetrievesFromDatabaseAndCaches()
    {
        // Arrange
        var products = new List<Product>
        {
            new() { Id = Guid.NewGuid(), Name = "Product 1", SKU = "SKU-001", UnitPrice = 10.00m },
            new() { Id = Guid.NewGuid(), Name = "Product 2", SKU = "SKU-002", UnitPrice = 20.00m }
        };

        var productDtos = new List<ProductDto>
        {
            new() { Id = products[0].Id, Name = "Product 1", SKU = "SKU-001", UnitPrice = 10.00m },
            new() { Id = products[1].Id, Name = "Product 2", SKU = "SKU-002", UnitPrice = 20.00m }
        };

        _cacheServiceMock
            .Setup(c => c.GetAsync<List<ProductDto>>("products_all", It.IsAny<CancellationToken>()))
            .ReturnsAsync((List<ProductDto>?)null);

        _productRepositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        _mapperMock
            .Setup(m => m.Map<List<ProductDto>>(products))
            .Returns(productDtos);

        // Act
        var result = await _productService.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(productDtos);

        _cacheServiceMock.Verify(c => c.GetAsync<List<ProductDto>>("products_all", It.IsAny<CancellationToken>()), Times.Once);
        _productRepositoryMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        _cacheServiceMock.Verify(c => c.SetAsync("products_all", productDtos, TimeSpan.FromMinutes(15), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithValidData_CreatesAndReturnsProduct()
    {
        // Arrange
        var createDto = new CreateProductDto
        {
            Name = "New Product",
            SKU = "NEW-001",
            Category = "Electronics",
            UnitPrice = 149.99m,
            Description = "A new product"
        };

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = createDto.Name,
            SKU = createDto.SKU,
            Category = createDto.Category,
            UnitPrice = createDto.UnitPrice,
            Description = createDto.Description
        };

        var productDto = new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            SKU = product.SKU,
            UnitPrice = product.UnitPrice
        };

        _createValidatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<CreateProductDto>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var emptyQueryable = new List<Product>().AsQueryable().BuildMock();
        _productRepositoryMock
            .Setup(r => r.Query())
            .Returns(emptyQueryable);

        _mapperMock
            .Setup(m => m.Map<Product>(createDto))
            .Returns(product);

        _mapperMock
            .Setup(m => m.Map<ProductDto>(product))
            .Returns(productDto);

        _unitOfWorkMock
            .Setup(u => u.Products.AddAsync(product, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _productService.CreateAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("New Product");
        result.SKU.Should().Be("NEW-001");

        _createValidatorMock.Verify(v => v.ValidateAsync(It.IsAny<ValidationContext<CreateProductDto>>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.Products.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _cacheServiceMock.Verify(c => c.RemoveAsync("products_all", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithValidData_UpdatesAndReturnsProduct()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var updateDto = new UpdateProductDto
        {
            Name = "Updated Product",
            UnitPrice = 199.99m
        };

        var existingProduct = new Product
        {
            Id = productId,
            Name = "Old Product",
            SKU = "OLD-001",
            UnitPrice = 99.99m
        };

        var updatedProductDto = new ProductDto
        {
            Id = productId,
            Name = "Updated Product",
            SKU = "OLD-001",
            UnitPrice = 199.99m
        };

        _updateValidatorMock
            .Setup(v => v.ValidateAsync(updateDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _unitOfWorkMock
            .Setup(u => u.Products.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProduct);

        _mapperMock
            .Setup(m => m.Map(updateDto, existingProduct))
            .Returns(existingProduct);

        _mapperMock
            .Setup(m => m.Map<ProductDto>(existingProduct))
            .Returns(updatedProductDto);

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _productService.UpdateAsync(productId, updateDto);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(productId);
        result.Name.Should().Be("Updated Product");
        result.UnitPrice.Should().Be(199.99m);

        _updateValidatorMock.Verify(v => v.ValidateAsync(It.IsAny<ValidationContext<UpdateProductDto>>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.Products.UpdateAsync(existingProduct, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _cacheServiceMock.Verify(c => c.RemoveAsync("products_all", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_DeletesProductAndReturnsTrue()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = new Product
        {
            Id = productId,
            Name = "Product to Delete",
            SKU = "DEL-001",
            UnitPrice = 50.00m
        };

        _unitOfWorkMock
            .Setup(u => u.Products.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _productService.DeleteAsync(productId);

        // Assert
        result.Should().BeTrue();

        _unitOfWorkMock.Verify(u => u.Products.DeleteAsync(product, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _cacheServiceMock.Verify(c => c.RemoveAsync("products_all", It.IsAny<CancellationToken>()), Times.Once);
        _cacheServiceMock.Verify(c => c.RemoveAsync($"product_{productId}", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ReturnsFalse()
    {
        // Arrange
        var productId = Guid.NewGuid();

        _unitOfWorkMock
            .Setup(u => u.Products.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _productService.DeleteAsync(productId);

        // Assert
        result.Should().BeFalse();

        _unitOfWorkMock.Verify(u => u.Products.DeleteAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}

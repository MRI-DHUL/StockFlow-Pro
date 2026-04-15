using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using StockFlow.Application.DTOs.Products;
using Xunit;

namespace StockFlow.IntegrationTests.Controllers;

public class ProductsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public ProductsControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllProducts_ReturnsSuccessStatusCode()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/products");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetAllProducts_ReturnsProductList()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/products");
        var products = await response.Content.ReadFromJsonAsync<List<ProductDto>>();

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        products.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateProduct_WithValidData_ReturnsCreated()
    {
        // Arrange
        var newProduct = new
        {
            Name = "Test Product",
            SKU = $"TEST-{Guid.NewGuid()}",
            Category = "Electronics",
            UnitPrice = 99.99,
            Description = "Test Description"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/products", newProduct);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task CreateProduct_WithInvalidData_ReturnsBadRequest()
    {
        // Arrange
        var invalidProduct = new
        {
            Name = "", // Invalid: empty name
            SKU = "",  // Invalid: empty SKU
            UnitPrice = -10 // Invalid: negative price
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/products", invalidProduct);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetProductById_ExistingProduct_ReturnsProduct()
    {
        // Arrange - Create a product first
        var newProduct = new
        {
            Name = "Test Product for Get",
            SKU = $"TEST-GET-{Guid.NewGuid()}",
            UnitPrice = 49.99
        };

        var createResponse = await _client.PostAsJsonAsync("/api/v1/products", newProduct);
        var createdProduct = await createResponse.Content.ReadFromJsonAsync<ProductDto>();

        // Act
        var response = await _client.GetAsync($"/api/v1/products/{createdProduct!.Id}");
        var product = await response.Content.ReadFromJsonAsync<ProductDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        product.Should().NotBeNull();
        product!.Id.Should().Be(createdProduct.Id);
        product.Name.Should().Be(newProduct.Name);
    }

    [Fact]
    public async Task GetProductById_NonExistingProduct_ReturnsNotFound()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/v1/products/{nonExistingId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateProduct_WithValidData_ReturnsNoContent()
    {
        // Arrange - Create a product first
        var newProduct = new
        {
            Name = "Original Name",
            SKU = $"TEST-UPDATE-{Guid.NewGuid()}",
            UnitPrice = 99.99
        };

        var createResponse = await _client.PostAsJsonAsync("/api/v1/products", newProduct);
        var createdProduct = await createResponse.Content.ReadFromJsonAsync<ProductDto>();

        var updatedProduct = new
        {
            Name = "Updated Name",
            SKU = createdProduct!.SKU,
            UnitPrice = 149.99,
            Category = "Updated Category"
        };

        // Act
        var response = await _client.PutAsJsonAsync(
            $"/api/v1/products/{createdProduct.Id}",
            updatedProduct);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteProduct_ExistingProduct_ReturnsNoContent()
    {
        // Arrange - Create a product first
        var newProduct = new
        {
            Name = "Product to Delete",
            SKU = $"TEST-DELETE-{Guid.NewGuid()}",
            UnitPrice = 29.99
        };

        var createResponse = await _client.PostAsJsonAsync("/api/v1/products", newProduct);
        var createdProduct = await createResponse.Content.ReadFromJsonAsync<ProductDto>();

        // Act
        var response = await _client.DeleteAsync($"/api/v1/products/{createdProduct!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify the product is deleted (soft delete)
        var getResponse = await _client.GetAsync($"/api/v1/products/{createdProduct.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

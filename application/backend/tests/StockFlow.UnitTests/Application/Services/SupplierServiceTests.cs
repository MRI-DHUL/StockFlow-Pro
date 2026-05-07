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
public class SupplierServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IRepository<Supplier>> _supplierRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IValidator<CreateSupplierDto>> _createValidatorMock;
    private readonly Mock<IValidator<UpdateSupplierDto>> _updateValidatorMock;
    private readonly SupplierService _supplierService;
    public SupplierServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _supplierRepositoryMock = new Mock<IRepository<Supplier>>();
        _mapperMock = new Mock<IMapper>();
        _createValidatorMock = new Mock<IValidator<CreateSupplierDto>>();
        _updateValidatorMock = new Mock<IValidator<UpdateSupplierDto>>();
        _supplierService = new SupplierService(
            _unitOfWorkMock.Object,
            _supplierRepositoryMock.Object,
            _mapperMock.Object,
            _createValidatorMock.Object,
            _updateValidatorMock.Object
        );
    }
    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsSupplier()
    {
        // Arrange
        var supplierId = Guid.NewGuid();
        var supplier = new Supplier
        {
            Id = supplierId,
            Name = "Test Supplier",
            ContactInfo = "John Doe",
            Email = "supplier@example.com",
            Phone = "1234567890",
            LeadTimeDays = 7
        };
        var supplierDto = new SupplierDto
        {
            Id = supplierId,
            Name = "Test Supplier",
            ContactInfo = "John Doe",
            Email = "supplier@example.com",
            Phone = "1234567890",
            LeadTimeDays = 7
        };
        _supplierRepositoryMock.Setup(r => r.GetByIdAsync(supplierId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(supplier);
        _mapperMock.Setup(m => m.Map<SupplierDto>(It.IsAny<Supplier>())).Returns(supplierDto);
        // Act
        var result = await _supplierService.GetByIdAsync(supplierId);
        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(supplierId);
        result.Name.Should().Be("Test Supplier");
        result.ContactInfo.Should().Be("John Doe");
        result.Email.Should().Be("supplier@example.com");
        result.LeadTimeDays.Should().Be(7);
        _supplierRepositoryMock.Verify(r => r.GetByIdAsync(supplierId, It.IsAny<CancellationToken>()), Times.Once);
        _mapperMock.Verify(m => m.Map<SupplierDto>(It.IsAny<Supplier>()), Times.Once);
    }
    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        var supplierId = Guid.NewGuid();
        _supplierRepositoryMock.Setup(r => r.GetByIdAsync(supplierId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Supplier?)null);
        // Act
        var result = await _supplierService.GetByIdAsync(supplierId);
        // Assert
        result.Should().BeNull();
        _supplierRepositoryMock.Verify(r => r.GetByIdAsync(supplierId, It.IsAny<CancellationToken>()), Times.Once);
        _mapperMock.Verify(m => m.Map<SupplierDto>(It.IsAny<Supplier>()), Times.Never);
    }
    [Fact]
    public async Task GetAllAsync_ReturnsAllSuppliers()
    {
        // Arrange
        var suppliers = new List<Supplier>
        {
            new() { Id = Guid.NewGuid(), Name = "Supplier 1", Email = "supplier1@example.com", LeadTimeDays = 7 },
            new() { Id = Guid.NewGuid(), Name = "Supplier 2", Email = "supplier2@example.com", LeadTimeDays = 14 }
        };
        var supplierDtos = new List<SupplierDto>
        {
            new() { Id = suppliers[0].Id, Name = "Supplier 1", Email = "supplier1@example.com", LeadTimeDays = 7 },
            new() { Id = suppliers[1].Id, Name = "Supplier 2", Email = "supplier2@example.com", LeadTimeDays = 14 }
        };
        var suppliersQueryable = suppliers;
        _supplierRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(suppliersQueryable);
        _mapperMock.Setup(m => m.Map<List<SupplierDto>>(It.IsAny<List<Supplier>>())).Returns(supplierDtos);
        // Act
        var result = await _supplierService.GetAllAsync();
        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(supplierDtos);
        _supplierRepositoryMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mapperMock.Verify(m => m.Map<List<SupplierDto>>(It.IsAny<List<Supplier>>()), Times.Once);
    }
    
    
    
    
    [Fact]
    public async Task DeleteAsync_WithInvalidId_ReturnsFalse()
    {
        // Arrange
        var supplierId = Guid.NewGuid();
        _supplierRepositoryMock.Setup(r => r.GetByIdAsync(supplierId, default(CancellationToken)))
            .ReturnsAsync((Supplier?)null);
        // Act
        var result = await _supplierService.DeleteAsync(supplierId);
        // Assert
        result.Should().BeFalse();
        _supplierRepositoryMock.Verify(r => r.GetByIdAsync(supplierId, default(CancellationToken)), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
    
    [Fact]
    public async Task GetAllAsync_WithNoSuppliers_ReturnsEmptyList()
    {
        // Arrange
        var emptyList = new List<Supplier>();
        _supplierRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(emptyList);
        var emptyDtoList = new List<SupplierDto>();
        _mapperMock.Setup(m => m.Map<List<SupplierDto>>(It.IsAny<List<Supplier>>())).Returns(emptyDtoList);
        // Act
        var result = await _supplierService.GetAllAsync();
        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }
}






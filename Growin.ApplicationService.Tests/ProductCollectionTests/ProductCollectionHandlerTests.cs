namespace Growin.ApplicationService.Tests.ProductCollectionTests;
using FluentAssertions;
using FunctionalConcepts.Errors;
using Growin.ApplicationService.Features.Products.Queries;
using Growin.ApplicationService.Features.Products.QueryHandlers;
using Growin.Domain.Features;
using Growin.Domain.Interfaces.WriteRepositories;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class ProductCollectionHandlerTests
{
    private readonly Mock<IProductReadRepository> _mockReadRepository;
    private readonly ProductCollectionHandler _handler;

    public ProductCollectionHandlerTests()
    {
        _mockReadRepository = new Mock<IProductReadRepository>();
        _handler = new ProductCollectionHandler(_mockReadRepository.Object);
    }

    [Test]
    public async Task ProductCollectionHandler_Handle_ShouldBeOk()
    {
        // Arrange
        var cancellationToken = new CancellationTokenSource().Token;
        var query = new ProductCollectionQuery();
        var productsDb = new List<Product>
        {
            CreateProduct(), CreateProduct(), CreateProduct(),
            CreateProduct(), CreateProduct(), CreateProduct()
        };

        _mockReadRepository.Setup(x => x.GetAll())
            .Returns(productsDb.AsQueryable());

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFail.Should().BeFalse();
        IQueryable<Product>? products = null;

        result.Then(prods => products = prods);
        products.Should().NotBeNullOrEmpty();
        products.Should().HaveCount(productsDb.Count);
        products.Should().BeEquivalentTo(productsDb);

        _mockReadRepository.Verify(x => x.GetAll());
        _mockReadRepository.VerifyNoOtherCalls();
    }

    [Test]
    public async Task ProductCollectionHandler_Handle_GetAllThrowsExn_ShouldBeError()
    {
        // Arrange
        var cancellationToken = new CancellationTokenSource().Token;
        var query = new ProductCollectionQuery();
        var exn = new InvalidDataException();

        _mockReadRepository.Setup(x => x.GetAll())
            .Throws(exn);

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFail.Should().BeTrue();
        BaseError? baseError = null;

        result.Else(err => baseError = err);
        baseError.Should().NotBeNull();
        baseError.Should()
            .BeOfType<UnhandledError>()
            .Subject.Message
            .Should()
            .Be("Handled exception on get all products");

        _mockReadRepository.Verify(x => x.GetAll());
        _mockReadRepository.VerifyNoOtherCalls();
    }

    private Product CreateProduct()
        => new ()
        {
            Id = 1,
            Name = "Sample Product",
            QuantityInStock = 100,
            CreatedAt = DateTime.UtcNow
        };
}

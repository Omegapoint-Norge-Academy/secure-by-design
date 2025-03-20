using AutoMapper;
using Contracts;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using SalesApi.Controllers;
using SalesApi.Domain.DomainPrimitives;
using SalesApi.Domain.Model;
using SalesApi.Domain.Services;
using SalesApi.Infrastructure;

namespace Tests.Unit;

public class ProductsControllerTests
{
    private readonly IMapper _mapper = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>()).CreateMapper();

    [Fact]
    public async Task GetById_ShouldReturn200_WhenAuthorized()
    {
        var productService = Substitute.For<IProductService>();
        var productId = new ProductId("no1");
        productService
            .GetWith(Arg.Any<ProductId>())
            .Returns(
                (
                    ReadDataResult.Success,
                    new Product(
                        productId,
                        new ProductName("Product 1"),
                        new Money(9m, "USD"),
                        new MarketId("no"))
                )
            );
        var controller = new ProductsController(productService, _mapper);

        var result = await controller.GetById(productId.ToString());

        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetById_ShouldReturnProductDTO_WhenAuthorized()
    {
        var productService = Substitute.For<IProductService>();
        var productId = new ProductId("no1");
        productService
            .GetWith(Arg.Any<ProductId>())
            .Returns(
                (
                    ReadDataResult.Success,
                    new Product(
                        productId,
                        new ProductName("Product 1"),
                        new Money(9m, "USD"),
                        new MarketId("no"))
                )
            );
        var controller = new ProductsController(productService, _mapper);

        var result = await controller.GetById(productId.ToString());

        Assert.IsType<ProductDTO>((result.Result as ObjectResult)?.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("no spaces")]
    [InlineData("thisisanidthatistoolong")]
    [InlineData("#")]
    [InlineData("<script>")]
    public async Task GetById_ShouldReturn400_WhenInvalidId(string invalidId)
    {
        var productService = Substitute.For<IProductService>();
        var controller = new ProductsController(productService, _mapper);

        var result = await controller.GetById(invalidId);

        Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Null(result.Value);
    }

    [Fact]
    public async Task GetById_ShouldReturn404_WhenNotFound()
    {
        var productService = Substitute.For<IProductService>();
        productService
            .GetWith(Arg.Any<ProductId>())
            .Returns((ReadDataResult.NotFound, null));
        var controller = new ProductsController(productService, _mapper);

        var result = await controller.GetById("no1");

        Assert.IsType<NotFoundResult>(result.Result);
        Assert.Null(result.Value);
    }

    [Fact]
    public async Task GetById_ShouldReturn403_WhenCanNotRead()
    {
        var productService = Substitute.For<IProductService>();
        productService
            .GetWith(Arg.Any<ProductId>())
            .Returns((ReadDataResult.NoAccessToOperation, null));
        var controller = new ProductsController(productService, _mapper);

        var result = await controller.GetById("no1");

        Assert.IsType<ForbidResult>(result.Result);
        Assert.Null(result.Value);
    }

    [Fact]
    public async Task GetById_ShouldReturn404_WhenNoAccessToData()
    {
        var productService = Substitute.For<IProductService>();
        productService
            .GetWith(Arg.Any<ProductId>())
            .Returns((ReadDataResult.NoAccessToData, null));
        var controller = new ProductsController(productService, _mapper);

        var result = await controller.GetById("no1");

        Assert.IsType<NotFoundResult>(result.Result);
        Assert.Null(result.Value);
    }
}
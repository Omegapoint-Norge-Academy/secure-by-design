using AutoMapper;
using Contracts;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SalesApi.Controllers;
using SalesApi.Domain.DomainPrimitives;
using SalesApi.Domain.Model;
using SalesApi.Domain.Services;
using SalesApi.Infrastructure;

namespace Tests;

[TestFixture]
public class ProductsControllerTests
{
    private readonly IMapper mapper = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>()).CreateMapper();

    [Test]
    public async Task GetById_ShouldReturn200_WhenAuthorized()
    {
        var productService = Mock.Of<IProductService>();

        var productId = new ProductId("no1");

        Mock.Get(productService)
            .Setup(service => service.GetWith(It.IsAny<ProductId>()))
            .ReturnsAsync((
                ReadDataResult.Success,
                new Product(
                    productId,
                    new ProductName("Product 1"),
                    new Money(9m, "USD"),
                    new MarketId("no"))
            ));

        var controller = new ProductsController(productService, mapper);

        var result = await controller.GetById(productId.ToString());

        Assert.IsInstanceOf(typeof(OkObjectResult), result.Result);
    }

    [Test]
    public async Task GetById_ShouldReturnProductDTO_WhenAuthorized()
    {
        var productService = Mock.Of<IProductService>();

        var productId = new ProductId("no1");

        Mock.Get(productService)
            .Setup(service => service.GetWith(It.IsAny<ProductId>()))
            .ReturnsAsync((
                ReadDataResult.Success,
                new Product(
                    productId,
                    new ProductName("Product 1"),
                    new Money(9m, "USD"),
                    new MarketId("no"))
            ));

        var controller = new ProductsController(productService, mapper);

        var result = await controller.GetById(productId.ToString());

        Assert.IsInstanceOf<ProductDTO>((result.Result as ObjectResult)?.Value);
    }

    [TestCase("")]
    [TestCase("no spaces")]
    [TestCase("thisisanidthatistoolong")]
    [TestCase("#")]
    [TestCase("<script>")]
    public async Task GetById_ShouldReturn400_WhenInvalidId(string invalidId)
    {
        var productService = Mock.Of<IProductService>();
        Mock.Get(productService)
            .Setup(service => service.GetWith(It.IsAny<ProductId>()))
            .ReturnsAsync((ReadDataResult.NoAccessToData, null));

        var controller = new ProductsController(productService, mapper);

        var result = await controller.GetById(invalidId);

        Assert.IsInstanceOf<BadRequestObjectResult>(result.Result);
        Assert.Null(result.Value);
    }

    [Test]
    public async Task GetById_ShouldReturn404_WhenNotFound()
    {
        var productService = Mock.Of<IProductService>();
        Mock.Get(productService)
            .Setup(service => service.GetWith(It.IsAny<ProductId>()))
            .ReturnsAsync((ReadDataResult.NotFound, null));

        var controller = new ProductsController(productService, mapper);

        var result = await controller.GetById("no1");

        Assert.IsInstanceOf<NotFoundResult>(result.Result);
        Assert.Null(result.Value);
    }

    [Test]
    public async Task GetById_ShouldReturn403_WhenCanNotRead()
    {
        var productService = Mock.Of<IProductService>();
        Mock.Get(productService)
            .Setup(service => service.GetWith(It.IsAny<ProductId>()))
            .ReturnsAsync((ReadDataResult.NoAccessToOperation, null));

        var controller = new ProductsController(productService, mapper);

        var result = await controller.GetById("no1");

        Assert.IsInstanceOf<ForbidResult>(result.Result);
        Assert.Null(result.Value);
    }

    [Test]
    public async Task GetById_ShouldReturn404_WhenNoAccessToData()
    {
        var productService = Mock.Of<IProductService>();
        Mock.Get(productService)
            .Setup(service => service.GetWith(It.IsAny<ProductId>()))
            .ReturnsAsync((ReadDataResult.NoAccessToData, null));

        var controller = new ProductsController(productService, mapper);

        var result = await controller.GetById("no1");

        Assert.IsInstanceOf<NotFoundResult>(result.Result);
        Assert.Null(result.Value);
    }
}

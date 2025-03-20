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
        // Implement this test
        Assert.Fail();
    }

    [Fact]
    public async Task GetById_ShouldReturnProductDTO_WhenAuthorized()
    {
        // Implement this test
        Assert.Fail();
    }

    [Theory]
    [InlineData("")]
    [InlineData("no spaces")]
    [InlineData("thisisanidthatistoolong")]
    [InlineData("#")]
    [InlineData("<script>")]
    public async Task GetById_ShouldReturn400_WhenInvalidId(string invalidId)
    {
        // Implement this test
        Assert.Fail();
    }

    [Fact]
    public async Task GetById_ShouldReturn404_WhenNotFound()
    {
        // Implement this test
        Assert.Fail();
    }

    [Fact]
    public async Task GetById_ShouldReturn403_WhenCanNotRead()
    {
        // Implement this test
        Assert.Fail();
    }

    [Fact]
    public async Task GetById_ShouldReturn404_WhenNoAccessToData()
    {
        // Implement this test
        Assert.Fail();
    }
}
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
        // Implement this test
    }

    [Test]
    public async Task GetById_ShouldReturnProductDTO_WhenAuthorized()
    {
        // Implement this test
    }

    [TestCase("")]
    [TestCase("no spaces")]
    [TestCase("thisisanidthatistoolong")]
    [TestCase("#")]
    [TestCase("<script>")]
    public async Task GetById_ShouldReturn400_WhenInvalidId(string invalidId)
    {
        // Implement this test
    }

    [Test]
    public async Task GetById_ShouldReturn404_WhenNotFound()
    {
        // Implement this test
    }

    [Test]
    public async Task GetById_ShouldReturn403_WhenCanNotRead()
    {
        // Implement this test
    }

    [Test]
    public async Task GetById_ShouldReturn404_WhenNoAccessToData()
    {
        // Implement this test
    }
}

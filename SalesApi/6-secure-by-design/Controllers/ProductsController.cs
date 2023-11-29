using AutoMapper;
using Contracts;
using Microsoft.AspNetCore.Mvc;
using SalesApi.Domain.DomainPrimitives;
using SalesApi.Domain.Model;
using SalesApi.Domain.Services;

namespace SalesApi.Controllers;

[ApiController]
[Route("api/product")]
public class ProductsController(IMapper mapper, IProductService productService) : ControllerBase
{
    [HttpGet("", Name = "GetProducts")]
    public async Task<ActionResult<IEnumerable<ProductDTO>>> GetAll()
    {
        var (result, products) = await productService.GetAllAvailableProducts();

        switch (result)
        {
            case ReadDataResult.NoAccessToOperation:
                return Forbid();
            case ReadDataResult.NotFound:
            case ReadDataResult.NoAccessToData:
                return NotFound();
            case ReadDataResult.InvalidData:
                return BadRequest("Invalid data.");
            case ReadDataResult.Success:
                return Ok(mapper.Map<IEnumerable<ProductDTO>>(products));
            default:
                throw new InvalidOperationException($"Result kind {result} is not supported");
        }
    }

    [HttpGet("{id}", Name = "GetProduct")]
    public async Task<ActionResult<IEnumerable<ProductDTO>>> GetById([FromRoute] string id)
    {
        if (!ProductId.IsValid(id))
        {
            return BadRequest("Id is not valid.");
        }

        var (result, product) = await productService.GetWith(new ProductId(id));

        switch (result)
        {
            case ReadDataResult.NoAccessToOperation:
                return Forbid();
            case ReadDataResult.NotFound:
            case ReadDataResult.NoAccessToData:
                return NotFound();
            case ReadDataResult.InvalidData:
                return BadRequest("Invalid data.");
            case ReadDataResult.Success:
                return Ok(mapper.Map<ProductDTO>(product));
            default:
                throw new InvalidOperationException($"Result kind {result} is not supported");
        }
    }
}

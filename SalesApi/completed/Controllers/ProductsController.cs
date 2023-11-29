using AutoMapper;
using Contracts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using SalesApi.Domain.DomainPrimitives;
using SalesApi.Domain.Model;
using SalesApi.Domain.Services;

namespace SalesApi.Controllers;

[ApiController]
[Route("api/product")]
public class ProductsController(IProductService productService, IMapper mapper) : ControllerBase
{
    private readonly IProductService _productService = productService;
    private readonly IMapper _mapper = mapper;

    [HttpGet("", Name = "GetProducts")]
    public async Task<ActionResult<IEnumerable<ProductDTO>>> GetAll()
    {
        var (result, products) = await _productService.GetAllAvailableProducts();

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
                return Ok(_mapper.Map<IEnumerable<ProductDTO>>(products));
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

        var (result, product) = await _productService.GetWith(new ProductId(id));

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
                return Ok(_mapper.Map<ProductDTO>(product));
            default:
                throw new InvalidOperationException($"Result kind {result} is not supported");
        }
    }

    [HttpPut("{id}", Name = "UpdateProductPrice")]
    public async Task<IActionResult> UpdatePriceById([FromRoute] string id, [FromBody] UpdatePriceRequest request)
    {
        if (!ProductId.IsValid(id)) return BadRequest("Id is not valid.");
        if (!Money.IsValid(request.Price, request.Currency)) return BadRequest("Price is not valid.");


        var (result, updatedProduct) = await _productService.UpdatePrice(new ProductId(id), new Money(request.Price, request.Currency));

        switch (result)
        {
            case WriteDataResult.NoAccessToOperation:
                return Forbid();
            case WriteDataResult.NotFound:
            case WriteDataResult.NoAccessToData:
                return NotFound();
            case WriteDataResult.InvalidData:
                return BadRequest("Invalid data.");
            case WriteDataResult.InvalidDomainOperation:
                return BadRequest("Invalid domain operation.");
            case WriteDataResult.Success:
                return Ok(_mapper.Map<ProductDTO>(updatedProduct));
            default:
                throw new InvalidOperationException($"Result kind {result} is not supported");
        }
    }

    [HttpPost(Name = "CreateNewProduct")]
    public async Task<IActionResult> CreateNewProduct([FromBody] CreateNewProductRequest request)
    {
        if (!Money.IsValid(request.Price, request.Currency)) return BadRequest("Price is not valid.");
        if (!MarketId.IsValid(request.MarketId)) return BadRequest("MarketId is not valid.");
        if (!ProductId.IsValid(request.MarketId)) return BadRequest("ProductId is not valid.");
        if (!ProductName.IsValid(request.Name)) return BadRequest("Name is not valid.");


        var (result, newProduct) = await productService.CreateNewProduct(new Product(
            new ProductId(request.MarketId),
            new ProductName(request.Name),
            new Money(request.Price, request.Currency),
            new MarketId(request.MarketId)));

        switch (result)
        {
            case WriteDataResult.NoAccessToOperation:
                return Forbid();
            case WriteDataResult.NotFound:
            case WriteDataResult.NoAccessToData:
                return NotFound();
            case WriteDataResult.InvalidData:
                return BadRequest("Invalid data.");
            case WriteDataResult.InvalidDomainOperation:
                return BadRequest("Invalid domain operation.");
            case WriteDataResult.Success:
                return Ok(_mapper.Map<ProductDTO>(newProduct));
            default:
                throw new InvalidOperationException($"Result kind {result} is not supported");
        }
    }
}
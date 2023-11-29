using AutoMapper;
using Contracts;
using Microsoft.AspNetCore.Mvc;
using SalesApi.Infrastructure;

namespace SalesApi.Controllers;

[ApiController]
[Route("api/product")]
public class ProductsController(IMapper mapper, IProductRepository productRepository) : ControllerBase
{
    [HttpGet("", Name = "GetProducts")]
    public async Task<ActionResult<IEnumerable<ProductDTO>>> GetAll()
    {
        return Ok((await productRepository.GetAllAvailable()).Select(mapper.Map<ProductDTO>));
    }

    [HttpGet("{id}", Name = "GetProduct")]
    public async Task<ActionResult<IEnumerable<ProductDTO>>> GetById([FromRoute] string id)
    {
        return Ok(mapper.Map<ProductDTO>(await productRepository.GetBy(id)));
    }
}

using Contracts;
using SalesClient.Client.Models;

namespace SalesClient.Client.Services;

public interface IProductService
{
    Task<List<ProductDTO>> GetAllAsync();

    Task<ProductDTO> UpdatePrice(string id, UpdatePriceModel updatePriceModel);
}
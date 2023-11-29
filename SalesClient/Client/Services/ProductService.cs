using System.Net.Http.Json;
using Contracts;
using SalesClient.Client.Models;

namespace SalesClient.Client.Services;

public class ProductService(HttpClient httpClient) : IProductService
{
    private readonly HttpClient _httpClient = httpClient;

    public Task<List<ProductDTO>> GetAllAsync()
    {
        return _httpClient.GetFromJsonAsync<List<ProductDTO>>("api/product");
    }

    public async Task<ProductDTO> UpdatePrice(string id, UpdatePriceModel updatePriceModel)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/product/{id}", new UpdatePriceRequest(updatePriceModel.Price, updatePriceModel.Currency));
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<ProductDTO>();
    }
}
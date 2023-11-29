using System.Reflection;
using AutoMapper;
using Contracts;
using SalesApi.Domain.Model;

namespace SalesApi.Infrastructure
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductDTO>();
        }
    }
}
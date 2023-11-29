using System.Reflection;
using AutoMapper;
using Contracts;
using SalesApi.Domain.DomainPrimitives;
using SalesApi.Domain.Model;

namespace SalesApi.Infrastructure
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            ApplyMappingsFromAssembly(Assembly.GetExecutingAssembly());

            CreateMap<Product, ProductDTO>()
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price.Value))
                .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.Price.Currency));
        }

        private void ApplyMappingsFromAssembly(Assembly assembly)
        {
            var types = assembly.GetExportedTypes()
                .Where(t => t.GetInterfaces().Any(i =>
                    i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapFrom<>)))
                .ToList();

            foreach (var type in types)
            {
                var instance = Activator.CreateInstance(type);

                var methodInfo = type.GetMethod("Mapping")
                                 ?? type.GetInterface("IMapFrom`1").GetMethod("Mapping");

                methodInfo?.Invoke(instance, new object[] { this });

            }
        }
    }
}
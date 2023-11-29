using AutoMapper;

namespace SalesApi.Infrastructure;
public interface IMapFrom<T>
{
    void Mapping(Profile profile) => profile.CreateMap(typeof(T), GetType());
}
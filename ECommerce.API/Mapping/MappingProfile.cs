using AutoMapper;
using ECommerce.Core.DTOs;
using ECommerce.Core.Entities;

namespace ECommerce.API.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductDto>().ReverseMap(); // iki yönlü
            CreateMap<Product, ProductResponseDto>();
        }
    }
}

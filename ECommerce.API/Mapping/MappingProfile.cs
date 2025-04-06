using AutoMapper;
using ECommerce.Core.DTOs;
using ECommerce.Core.Entities;

namespace ECommerce.API.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Ürün
            CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<Product, ProductResponseDto>();

            // Kullanıcı
            CreateMap<User, UserDto>();

            // Sepet
            CreateMap<CartItem, CartItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Product.Price))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User));

            // Sipariş
            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name));

            CreateMap<Order, OrderResponseDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.OrderItems))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User));

        }
    }
}

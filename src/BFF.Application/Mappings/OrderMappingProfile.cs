using AutoMapper;
using BFF.Application.DTOs.GraphQL;
using BFF.Application.DTOs.Requests;
using BFF.Application.DTOs.Responses;
using BFF.Domain.Models;

namespace BFF.Application.Mappings;

public class OrderMappingProfile : Profile
{
    public OrderMappingProfile()
    {
        // GraphQL to Domain
        CreateMap<OrderGraphQL, Order>();
        CreateMap<OrderItemGraphQL, OrderItem>();
        
        // GraphQL to Response (Direct mapping)
        CreateMap<OrderGraphQL, OrderResponse>();
        CreateMap<OrderItemGraphQL, OrderItemResponse>();
        
        // Domain to Response
        CreateMap<Order, OrderResponse>();
        CreateMap<OrderItem, OrderItemResponse>();
        
        // Request to Domain
        CreateMap<CreateOrderRequest, Order>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "Pending"))
            .ForMember(dest => dest.TotalAmount, opt => opt.Ignore());

        CreateMap<OrderItemRequest, OrderItem>()
            .ForMember(dest => dest.Subtotal, opt => opt.MapFrom(src => src.Quantity * src.Price));
    }
}

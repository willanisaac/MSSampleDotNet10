using AutoMapper;
using BFF.Application.DTOs.GraphQL;
using BFF.Application.DTOs.Requests;
using BFF.Application.DTOs.Responses;
using BFF.Domain.Models;

namespace BFF.Application.Mappings;

public class PaymentMappingProfile : Profile
{
    public PaymentMappingProfile()
    {
        // GraphQL to Domain
        CreateMap<PaymentGraphQL, Payment>();
        
        // Domain to Response
        CreateMap<Payment, PaymentResponse>();
        
        // Request to Domain
        CreateMap<ProcessPaymentRequest, Payment>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "Pending"))
            .ForMember(dest => dest.TransactionId, opt => opt.Ignore());
    }
}

using AutoMapper;
using BFF.Application.DTOs.GraphQL;
using BFF.Application.DTOs.Requests;
using BFF.Application.DTOs.Responses;
using BFF.Domain.Models;

namespace BFF.Application.Mappings;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        // GraphQL to Domain
        CreateMap<UserGraphQL, User>();
        
        // Domain to Response
        CreateMap<User, UserResponse>();
        
        // Request to Domain
        CreateMap<CreateUserRequest, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => "Customer"))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

        CreateMap<UpdateUserRequest, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Role, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore());
    }
}

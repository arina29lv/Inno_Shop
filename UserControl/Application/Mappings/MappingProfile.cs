using AutoMapper;
using UserControl.Application.Commands.AuthCommands;
using UserControl.Application.Commands.UserCommands;
using UserControl.Application.DTOs.UserDTOs;
using UserControl.Domain.Models;

namespace UserControl.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDto>();
        CreateMap<UserDto, User>();
        CreateMap<UpdateUserCommand, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
        CreateMap<RegisterUserCommand, User>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()) 
            .ForMember(dest => dest.EmailConfirmationToken, opt => opt.Ignore()); 
    }
}
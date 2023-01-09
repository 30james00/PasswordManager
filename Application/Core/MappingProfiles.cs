using AutoMapper;
using Domain;
using PasswordManager.Application.IpAddressBlocks.DTOs;
using PasswordManager.Application.SavedPasswords.DTOs;
using PasswordManager.Application.SharedPasswords.DTOs;

namespace PasswordManager.Application.Core;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<SharedPassword, SharedPasswordMiniDto>()
            .ForMember(x => x.Login, o => o.MapFrom(x => x.Account.Login));

        CreateMap<SavedPassword, SavedPasswordDto>()
            .ForMember(x => x.SharedTo, o => o.MapFrom(x => x.SharedPasswords.ToList()));

        CreateMap<SharedPassword, SharedPasswordDto>()
            .ForMember(x => x.Owner, o => o.MapFrom(x => x.SavedPassword.Account.Login))
            .ForMember(x => x.Login, o => o.MapFrom(x => x.SavedPassword.Login))
            .ForMember(x => x.WebAddress, o => o.MapFrom(x => x.SavedPassword.WebAddress))
            .ForMember(x => x.Description, o => o.MapFrom(x => x.SavedPassword.Description));

        CreateMap<IpAddressBlock, IpAddressBlockDto>();
    }
}
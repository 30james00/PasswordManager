using AutoMapper;
using Domain;
using PasswordManager.Application.IpAddressBlocks.DTOs;
using PasswordManager.Application.SavedPasswords.DTOs;

namespace PasswordManager.Application.Core;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<SavedPassword, SavedPasswordDto>();
        CreateMap<IpAddressBlock, IpAddressBlockDto>();
    }
}
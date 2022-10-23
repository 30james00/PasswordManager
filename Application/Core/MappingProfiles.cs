using AutoMapper;
using Domain;
using PasswordManager.Application.SavedPasswords.DTOs;

namespace PasswordManager.Application.Core;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<SavedPassword, SavedPasswordDto>();
    }
}
using AutoMapper;
using Domain;
using PasswordManager.Application.SavedPasswords;

namespace PasswordManager.Application;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<SavedPassword, SavedPasswordDto>();
    }
}
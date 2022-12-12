using AutoMapper;
using Domain;
using PasswordManager.Application.SavedPasswords.DAOs;

namespace PasswordManager.Application.Core;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<SavedPassword, SavedPasswordDao>();
    }
}
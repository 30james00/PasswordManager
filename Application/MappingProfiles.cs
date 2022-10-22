using AutoMapper;
using Domain;
using PasswordManager.Application.SavedPasswords;
using PasswordManager.Application.SavedPasswords.DTOs;

namespace PasswordManager.Application;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<SavedPassword, SavedPasswordDto>();
    }
}
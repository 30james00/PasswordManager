using PasswordManager.Application.SavedPasswords.DTOs;

namespace PasswordManager.Application.SavedPasswords;

public interface ISavedPasswordService
{
    public Task<List<SavedPasswordDto>> ListPassword();
    public Task<SavedPasswordDto?> DetailPassword(Guid id);
    public Task<string> DecryptPassword();
    public Task<SavedPasswordDto> CreatePassword();
    public Task<SavedPasswordDto> EditPassword();
    public Task DeletePassword();
}
using PasswordManager.Application.SavedPasswords.DTOs;

namespace PasswordManager.Application.SavedPasswords;

public interface ISavedPasswordService
{
    public Task<List<SavedPasswordDto>> ListPassword();
    public Task<SavedPasswordDto?> DetailPassword(Guid id);
    public Task<SavedPasswordDto> CreatePassword(CreatePasswordDto passwordDto);
    public Task<SavedPasswordDto> EditPassword(EditPasswordDto passwordDto);
    public Task DeletePassword(Guid id);
    public Task<string> DecryptPassword(Guid id);
}
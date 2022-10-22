namespace PasswordManager.Application.SavedPasswords;

public interface ISavedPasswordService
{
    public Task<List<SavedPasswordDto>> ListPassword();
    public Task<string> DecryptPassword();
    public Task<SavedPasswordDto> CreatePassword();
    public Task<SavedPasswordDto> EditPassword();
    public Task DeletePassword();
}
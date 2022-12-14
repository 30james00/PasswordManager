using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Domain;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PasswordManager.Application.Accounts;
using PasswordManager.Application.Core;
using PasswordManager.Application.SavedPasswords.DTOs;
using PasswordManager.Application.Security.Crypto;
using PasswordManager.Application.Security.Token;

namespace PasswordManager.Application.SavedPasswords;

public class CreatePasswordCommand : IRequest<ApiResult<SavedPasswordDto>>
{
    [Required] public string Password { get; set; } = null!;
    [Required] public string WebAddress { get; set; } = null!;
    public string? Description { get; set; }
    public string? Login { get; set; }
}

public class CreatePasswordCommandHandler : IRequestHandler<CreatePasswordCommand, ApiResult<SavedPasswordDto>>
{
    private readonly DataContext _dataContext;
    private readonly IUserAccessor _userAccessor;
    private readonly IAccountService _accountService;
    private readonly ICryptoService _cryptoService;
    private readonly IMapper _mapper;

    public CreatePasswordCommandHandler(DataContext dataContext, IUserAccessor userAccessor,
        IAccountService accountService, ICryptoService cryptoService, IMapper mapper)
    {
        _dataContext = dataContext;
        _userAccessor = userAccessor;
        _accountService = accountService;
        _cryptoService = cryptoService;
        _mapper = mapper;
    }

    public async Task<ApiResult<SavedPasswordDto>> Handle(CreatePasswordCommand request,
        CancellationToken cancellationToken)
    {
        // Get Account
        var accountId = _userAccessor.GetUserId();
        if (accountId == null) return ApiResult<SavedPasswordDto>.Forbidden();
        var account =
            await _dataContext.Accounts.FirstOrDefaultAsync(x => x.Id == Guid.Parse(accountId), cancellationToken);
        if (account == null) return ApiResult<SavedPasswordDto>.Forbidden();

        //Create key for password encryption using MasterPassword hash
        var key = _accountService.GetMasterPasswordKey(account.PasswordHash);
        var savedPassword = new SavedPassword
        {
            // Encrypt password
            Password = _cryptoService.Encrypt(request.Password, key, out var ivBytes),
            WebAddress = request.WebAddress,
            Description = request.Description,
            Login = request.Login,
            Iv = Convert.ToBase64String(ivBytes),
            AccountId = Guid.Parse(accountId),
        };

        await _dataContext.SavedPasswords.AddAsync(savedPassword, cancellationToken);
        var result = await _dataContext.SaveChangesAsync(cancellationToken) > 0;
        return result
            ? ApiResult<SavedPasswordDto>.Success(_mapper.Map<SavedPassword, SavedPasswordDto>(savedPassword))
            : ApiResult<SavedPasswordDto>.Failure("Error saving new Password to Database");
    }
}
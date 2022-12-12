using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PasswordManager.Application.Accounts;
using PasswordManager.Application.Core;
using PasswordManager.Application.SavedPasswords.DAOs;
using PasswordManager.Application.Security.Crypto;
using PasswordManager.Application.Security.Token;

namespace PasswordManager.Application.SavedPasswords;

public record ListPasswordQuery() : IRequest<ApiResult<List<SavedPasswordDao>>>;

public class ListPasswordQueryHandler : IRequestHandler<ListPasswordQuery, ApiResult<List<SavedPasswordDao>>>
{
    private readonly DataContext _dataContext;
    private readonly IUserAccessor _userAccessor;
    private readonly IMapper _mapper;

    public ListPasswordQueryHandler(DataContext dataContext, IUserAccessor userAccessor, IMapper mapper)
    {
        _dataContext = dataContext;
        _userAccessor = userAccessor;
        _mapper = mapper;
    }

    public async Task<ApiResult<List<SavedPasswordDao>>> Handle(ListPasswordQuery request,
        CancellationToken cancellationToken)
    {
        // Get Account
        var accountId = _userAccessor.GetUserId();
        if (accountId == null) return ApiResult<List<SavedPasswordDao>>.Forbidden();

        return ApiResult<List<SavedPasswordDao>>.Success(await _dataContext.SavedPasswords
            .Where(x => x.AccountId == Guid.Parse(accountId))
            .ProjectTo<SavedPasswordDao>(_mapper.ConfigurationProvider).ToListAsync(cancellationToken));
    }
}
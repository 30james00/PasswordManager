using MediatR;
using PasswordManager.Application.Core;

namespace PasswordManager.Application.SharedPasswords;

public record CreateSharedPasswordCommand() : IRequest<ApiResult<Unit>>;
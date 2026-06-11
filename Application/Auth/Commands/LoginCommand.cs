using GestaoPedidosAPI.Application.Auth.DTOs;
using MediatR;

namespace GestaoPedidosAPI.Application.Auth.Commands;

public record LoginCommand(string Email, string Password) : IRequest<LoginResponse>;

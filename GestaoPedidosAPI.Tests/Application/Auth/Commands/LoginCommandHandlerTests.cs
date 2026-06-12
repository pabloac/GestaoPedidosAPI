using FluentAssertions;
using GestaoPedidosAPI.Application.Auth.Commands;
using GestaoPedidosAPI.Application.Common.Interfaces;
using GestaoPedidosAPI.Domain.Entities;
using Moq;

namespace GestaoPedidosAPI.Tests.Application.Auth.Commands;

public class LoginCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IJwtTokenGenerator> _jwtTokenGeneratorMock = new();
    private readonly Mock<IPasswordHasher> _passwordHasherMock = new();
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _handler = new LoginCommandHandler(
            _userRepositoryMock.Object,
            _jwtTokenGeneratorMock.Object,
            _passwordHasherMock.Object);
    }

    [Fact]
    public async Task Handle_CredenciaisValidas_RetornaTokenEEmail()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), "dev@martech.com", "hash");
        var command = new LoginCommand("dev@martech.com", "Senha@123");

        _userRepositoryMock
            .Setup(r => r.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordHasherMock
            .Setup(p => p.Verify(command.Password, user.PasswordHash))
            .Returns(true);

        _jwtTokenGeneratorMock
            .Setup(j => j.GenerateToken(user))
            .Returns("token-gerado");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Token.Should().Be("token-gerado");
        result.Email.Should().Be("dev@martech.com");
    }

    [Fact]
    public async Task Handle_UsuarioNaoEncontrado_LancaUnauthorizedAccessException()
    {
        // Arrange
        var command = new LoginCommand("naoexiste@email.com", "senha");

        _userRepositoryMock
            .Setup(r => r.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task Handle_SenhaIncorreta_LancaUnauthorizedAccessException()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), "dev@martech.com", "hash");
        var command = new LoginCommand("dev@martech.com", "senha-errada");

        _userRepositoryMock
            .Setup(r => r.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordHasherMock
            .Setup(p => p.Verify(command.Password, user.PasswordHash))
            .Returns(false);

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }
}

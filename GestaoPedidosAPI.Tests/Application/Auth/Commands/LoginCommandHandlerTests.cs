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
    public async Task Handle_CredenciaisValidas_TokenNaoEhVazio()
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
        result.Token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Handle_CredenciaisValidas_ChamaGenerateTokenUmaVez()
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
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _jwtTokenGeneratorMock.Verify(j => j.GenerateToken(user), Times.Once);
    }

    [Fact]
    public async Task Handle_CredenciaisValidas_EmailDoResponseBateComOUsuario()
    {
        // Arrange
        var email = "outro@email.com";
        var user = new User(Guid.NewGuid(), email, "hash");
        var command = new LoginCommand(email, "qualquer-senha");

        _userRepositoryMock
            .Setup(r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordHasherMock
            .Setup(p => p.Verify(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(true);

        _jwtTokenGeneratorMock
            .Setup(j => j.GenerateToken(It.IsAny<User>()))
            .Returns("token");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Email.Should().Be(email);
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
    public async Task Handle_UsuarioNaoEncontrado_NaoChamaVerifyNemGenerateToken()
    {
        // Arrange
        var command = new LoginCommand("naoexiste@email.com", "senha");

        _userRepositoryMock
            .Setup(r => r.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        try { await _handler.Handle(command, CancellationToken.None); } catch { }

        // Assert
        _passwordHasherMock.Verify(p => p.Verify(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _jwtTokenGeneratorMock.Verify(j => j.GenerateToken(It.IsAny<User>()), Times.Never);
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

    [Fact]
    public async Task Handle_SenhaIncorreta_NaoChamaGenerateToken()
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
        try { await _handler.Handle(command, CancellationToken.None); } catch { }

        // Assert
        _jwtTokenGeneratorMock.Verify(j => j.GenerateToken(It.IsAny<User>()), Times.Never);
    }

    [Theory]
    [InlineData("", "Senha@123")]
    [InlineData("dev@martech.com", "")]
    [InlineData("", "")]
    public async Task Handle_DadosVazios_LancaUnauthorizedAccessException(string email, string password)
    {
        // Arrange
        var command = new LoginCommand(email, password);

        _userRepositoryMock
            .Setup(r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Theory]
    [InlineData("EMAIL_ERRADO", "Senha@123")]
    [InlineData("semdominio@", "Senha@123")]
    [InlineData("@semlocal.com", "Senha@123")]
    public async Task Handle_EmailSemFormatoValido_UsuarioNaoEncontrado_LancaUnauthorized(string email, string password)
    {
        // Arrange
        var command = new LoginCommand(email, password);

        _userRepositoryMock
            .Setup(r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }
}

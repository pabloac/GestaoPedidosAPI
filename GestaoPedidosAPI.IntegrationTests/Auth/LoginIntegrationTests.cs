using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GestaoPedidosAPI.Application.Auth.Commands;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace GestaoPedidosAPI.IntegrationTests.Auth;

public class LoginIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public LoginIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((_, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] =
                        "Data Source=C:\\projetos\\GestaoPedidosAPI\\gestao_pedidos.db"
                });
            });
        })
        .CreateClient();
    }

    [Fact]
    public async Task Login_CredenciaisValidas_Retorna200ComTokenEEmail()
    {
        // Arrange
        var body = new { email = "dev@martech.com", password = "Senha@123" };

        // Act
        var response = await _client.PostAsJsonAsync("/auth/login", body);
        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result!.Token.Should().NotBeNullOrEmpty();
        result.Email.Should().Be("dev@martech.com");
    }

    [Fact]
    public async Task Login_SenhaErrada_Retorna401()
    {
        // Arrange
        var body = new { email = "dev@martech.com", password = "senha-errada" };

        // Act
        var response = await _client.PostAsJsonAsync("/auth/login", body);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_UsuarioInexistente_Retorna401()
    {
        // Arrange
        var body = new { email = "naoexiste@email.com", password = "Senha@123" };

        // Act
        var response = await _client.PostAsJsonAsync("/auth/login", body);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_EmailVazio_Retorna400()
    {
        // Arrange
        var body = new { email = "", password = "Senha@123" };

        // Act
        var response = await _client.PostAsJsonAsync("/auth/login", body);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_SenhaVazia_Retorna400()
    {
        // Arrange
        var body = new { email = "dev@martech.com", password = "" };

        // Act
        var response = await _client.PostAsJsonAsync("/auth/login", body);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_BodyVazio_Retorna400()
    {
        // Arrange
        var body = new { };

        // Act
        var response = await _client.PostAsJsonAsync("/auth/login", body);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}

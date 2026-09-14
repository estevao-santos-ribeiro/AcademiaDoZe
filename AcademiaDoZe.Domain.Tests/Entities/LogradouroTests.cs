// Estevão Santos Ribeiro
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Tests.Entities;

public class LogradouroTests
{
    [Theory(DisplayName = "Logradouro: nome vazio -> NOME_OBRIGATORIO")]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Deve_Falhar_Criacao_Quando_NomeVazio(string? nome)
    {
        var result = Logradouro.Criar(1, "12345-678", nome!, "Bairro", "Cidade", "SP", "Brasil");
        Assert.True(result.IsFailure);
        Assert.NotEmpty(result.Notifications);
        Assert.Contains(result.Notifications, n => n.Mensagem == "NOME_OBRIGATORIO");
    }

    [Theory(DisplayName = "Logradouro: normaliza estado removendo espaços e upper")]
    [InlineData(" s p ", "SP")]
    [InlineData(" sp ", "SP")]
    [InlineData("Sp", "SP")]
    [InlineData("  sc  ", "SC")]
    public void Deve_Normalizar_Estado_Quando_InputContemEspacos(string inputEstado, string expected)
    {
        var result = Logradouro.Criar(1, "12345-678", "Rua", "Bairro", "Cidade", inputEstado, "Brasil");
        Assert.True(result.IsSuccess);
        Assert.Equal(expected, result.Value!.Estado);
    }

    [Theory(DisplayName = "Logradouro: campos obrigatórios vazios -> mensagens específicas")]
    [InlineData("", "Bairro", "Cidade", "SP", "Brasil", "NOME_OBRIGATORIO")]
    [InlineData("Rua", "", "Cidade", "SP", "Brasil", "BAIRRO_OBRIGATORIO")]
    [InlineData("Rua", "Bairro", "", "SP", "Brasil", "CIDADE_OBRIGATORIO")]
    [InlineData("Rua", "Bairro", "Cidade", "", "Brasil", "ESTADO_OBRIGATORIO")]
    [InlineData("Rua", "Bairro", "Cidade", "SP", "", "PAIS_OBRIGATORIO")]
    public void Deve_Falhar_Criacao_Quando_CamposObrigatoriosVazios(string rua, string bairro, string cidade, string estado, string pais, string expected)
    {
        var result = Logradouro.Criar(1, "12345-678", rua, bairro, cidade, estado, pais);
        Assert.True(result.IsFailure);
        Assert.NotEmpty(result.Notifications);
        Assert.Contains(result.Notifications, n => n.Mensagem == expected);
    }
}

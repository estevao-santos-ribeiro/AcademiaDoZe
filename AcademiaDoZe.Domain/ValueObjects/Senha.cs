// Estevão Santos Ribeiro
namespace AcademiaDoZe.Domain.ValueObjects;

public record Senha
{
    public string Valor { get; }

    private Senha(string valor)
    {
        Valor = valor;
    }

    public static Senha Criar(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new Exception("SENHA_INVALIDA");

        if (valor.Length < 6)
            throw new Exception("SENHA_INVALIDA");

        return new Senha(valor);
    }
}

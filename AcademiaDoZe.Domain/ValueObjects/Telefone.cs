// Estevão Santos Ribeiro
namespace AcademiaDoZe.Domain.ValueObjects;

public record Telefone
{
    public string Valor { get; }

    private Telefone(string valor)
    {
        Valor = valor;
    }

    public static Telefone Criar(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new Exception("TELEFONE_INVALIDO");

        var digitos = new string(valor.Where(char.IsDigit).ToArray());

        if (digitos.Length is not (10 or 11))
            throw new Exception("TELEFONE_INVALIDO");

        return new Telefone(digitos);
    }

    public override string ToString() => Valor;
}

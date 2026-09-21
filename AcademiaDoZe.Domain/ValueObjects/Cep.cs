// Estevão Santos Ribeiro
namespace AcademiaDoZe.Domain.ValueObjects;

public record Cep
{
    public string Valor { get; }

    private Cep(string valor)
    {
        Valor = valor;
    }

    public static Cep Criar(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new Exception("CEP_INVALIDO");

        var digitos = new string(valor.Where(char.IsDigit).ToArray());

        if (digitos.Length != 8)
            throw new Exception("CEP_INVALIDO");

        return new Cep(digitos);
    }

    public override string ToString() => Valor;
}

// Estevão Santos Ribeiro
using System.Text.RegularExpressions;

namespace AcademiaDoZe.Domain.ValueObjects;

public partial record Email
{
    public string Valor { get; }

    private Email(string valor)
    {
        Valor = valor;
    }

    public static Email Criar(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new Exception("EMAIL_INVALIDO");

        if (!EmailRegex().IsMatch(valor))
            throw new Exception("EMAIL_INVALIDO");

        return new Email(valor);
    }

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex EmailRegex();

    public override string ToString() => Valor;
}

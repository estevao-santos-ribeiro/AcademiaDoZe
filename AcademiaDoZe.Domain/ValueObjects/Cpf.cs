// Estevão Santos Ribeiro
namespace AcademiaDoZe.Domain.ValueObjects;

public record Cpf
{
    public string Valor { get; }

    private Cpf(string valor)
    {
        Valor = valor;
    }

    public static Cpf Criar(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new Exception("CPF_INVALIDO");

        var digitos = new string(valor.Where(char.IsDigit).ToArray());

        if (digitos.Length != 11)
            throw new Exception("CPF_INVALIDO");

        if (new string(digitos[0], 11) == digitos)
            throw new Exception("CPF_INVALIDO");

        if (!ValidarDigitosVerificadores(digitos))
            throw new Exception("CPF_INVALIDO");

        return new Cpf(digitos);
    }

    private static bool ValidarDigitosVerificadores(string cpf)
    {
        var multiplicadores1 = new[] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        var multiplicadores2 = new[] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

        var soma = 0;
        for (var i = 0; i < 9; i++)
            soma += (cpf[i] - '0') * multiplicadores1[i];

        var resto = soma % 11;
        var digito1 = resto < 2 ? 0 : 11 - resto;

        soma = 0;
        for (var i = 0; i < 10; i++)
            soma += (cpf[i] - '0') * multiplicadores2[i];

        resto = soma % 11;
        var digito2 = resto < 2 ? 0 : 11 - resto;

        return cpf[9] - '0' == digito1 && cpf[10] - '0' == digito2;
    }

    public override string ToString() => Valor;
}

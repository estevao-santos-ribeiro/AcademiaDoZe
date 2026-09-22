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
}

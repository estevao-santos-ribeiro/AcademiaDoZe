// Estevão Santos Ribeiro
using AcademiaDoZe.Domain.Entities;

namespace AcademiaDoZe.Domain.ValueObjects;

public record Endereco
{
    public Logradouro Logradouro { get; }
    public string Numero { get; }
    public string? Complemento { get; }

    private Endereco(Logradouro logradouro, string numero, string? complemento)
    {
        Logradouro = logradouro;
        Numero = numero;
        Complemento = complemento;
    }
}

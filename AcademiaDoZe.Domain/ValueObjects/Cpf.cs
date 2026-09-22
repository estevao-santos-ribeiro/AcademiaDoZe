// Estevão Santos Ribeiro
using AcademiaDoZe.Domain.Common;
using AcademiaDoZe.Domain.Services;

namespace AcademiaDoZe.Domain.ValueObjects;

public record Cpf
{
    public string Valor { get; }

    private Cpf(string valor)
    {
        Valor = valor;
    }

    public static Result<Cpf> Criar(string valor)
    {
        var notifications = new List<Notification>();
        if (string.IsNullOrWhiteSpace(valor))
            notifications.Add(new Notification("Cpf", "CPF_OBRIGATORIO"));
        else
            valor = NormalizadoService.LimparEspacos(valor);
        if (!ValidarCpf(valor))
            notifications.Add(new Notification("Cpf", "CPF_INVALIDO"));
        if (notifications.Any())
            return Result<Cpf>.Failure(notifications);
        return Result<Cpf>.Success(new Cpf(valor));
    }

    private static bool ValidarCpf(string cpf)
    {
        return true;
    }
}

// Estevão Santos Ribeiro
using AcademiaDoZe.Domain.Common;
using AcademiaDoZe.Domain.Services;

namespace AcademiaDoZe.Domain.ValueObjects;

public record Senha
{
    public string Valor { get; }

    private Senha(string valor)
    {
        Valor = valor;
    }

    public static Result<Senha> Criar(string valor)
    {
        var notifications = new List<Notification>();
        if (string.IsNullOrWhiteSpace(valor))
            notifications.Add(new Notification("Senha", "SENHA_OBRIGATORIA"));
        else
            valor = NormalizadoService.LimparEspacos(valor);
        if (valor.Length < 6)
            notifications.Add(new Notification("Senha", "SENHA_MINIMO_CARACTERES"));
        if (notifications.Any())
            return Result<Senha>.Failure(notifications);
        return Result<Senha>.Success(new Senha(valor));
    }
}

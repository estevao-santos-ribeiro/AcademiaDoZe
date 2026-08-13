// Estevão Santos Ribeiro

using AcademiaDoZe.Domain.Common;
using AcademiaDoZe.Domain.Services;

namespace AcademiaDoZe.Domain.ValueObjects
{
    public sealed record Cpf
    {
        private Cpf(string numero)
        {
            Numero = numero;
        }

        public string Numero { get; }

        public static Result<Cpf> Criar(string? numero)
        {
            var notifications = new List<Notification>();

            var normalizado = Normalizer.NormalizeCpf(numero);

            if (string.IsNullOrWhiteSpace(normalizado))
            {
                notifications.Add(Notification.Create(nameof(Cpf), "CPF é obrigatório."));
            }
            else if (normalizado.Length != 11)
            {
                notifications.Add(Notification.Create(nameof(Cpf), "CPF deve conter exatamente 11 dígitos."));
            }
            else if (!normalizado.All(char.IsDigit))
            {
                notifications.Add(Notification.Create(nameof(Cpf), "CPF deve conter apenas dígitos."));
            }

            if (notifications.Any())
            {
                return Result<Cpf>.Failure(notifications);
            }

            return Result<Cpf>.Success(new Cpf(normalizado));
        }
    }
}

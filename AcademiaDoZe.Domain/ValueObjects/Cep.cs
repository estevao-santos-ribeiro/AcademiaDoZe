// Estevão Santos Ribeiro

using AcademiaDoZe.Domain.Common;
using AcademiaDoZe.Domain.Services;

namespace AcademiaDoZe.Domain.ValueObjects
{
    public sealed record Cep
    {
        private Cep(string codigo)
        {
            Codigo = codigo;
        }

        public string Codigo { get; }

        public static Result<Cep> Criar(string? codigo)
        {
            var notifications = new List<Notification>();

            var normalizado = Normalizer.NormalizeCep(codigo);

            if (string.IsNullOrWhiteSpace(normalizado))
            {
                notifications.Add(Notification.Create(nameof(Cep), "CEP é obrigatório."));
            }
            else if (normalizado.Length != 8)
            {
                notifications.Add(Notification.Create(nameof(Cep), "CEP deve conter exatamente 8 dígitos."));
            }
            else if (!normalizado.All(char.IsDigit))
            {
                notifications.Add(Notification.Create(nameof(Cep), "CEP deve conter apenas dígitos."));
            }

            if (notifications.Any())
            {
                return Result<Cep>.Failure(notifications);
            }

            return Result<Cep>.Success(new Cep(normalizado));
        }
    }
}

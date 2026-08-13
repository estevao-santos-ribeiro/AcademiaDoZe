// Estevão Santos Ribeiro

using System.Text.RegularExpressions;
using AcademiaDoZe.Domain.Common;
using AcademiaDoZe.Domain.Services;

namespace AcademiaDoZe.Domain.ValueObjects
{
    public sealed record Email
    {
        private Email(string endereco)
        {
            Endereco = endereco;
        }

        public string Endereco { get; }

        public static Result<Email> Criar(string? endereco)
        {
            var notifications = new List<Notification>();

            var normalizado = Normalizer.NormalizeEmail(endereco);

            if (string.IsNullOrWhiteSpace(normalizado))
            {
                notifications.Add(Notification.Create(nameof(Email), "E-mail é obrigatório."));
            }
            else if (!normalizado.Contains('@'))
            {
                notifications.Add(Notification.Create(nameof(Email), "E-mail deve conter um '@'."));
            }
            else if (!IsValidEmailFormat(normalizado))
            {
                notifications.Add(Notification.Create(nameof(Email), "E-mail possui formato inválido."));
            }

            if (notifications.Any())
            {
                return Result<Email>.Failure(notifications);
            }

            return Result<Email>.Success(new Email(normalizado));
        }

        private static bool IsValidEmailFormat(string email)
        {
            var pattern = @"^[^\s@]+@[^\s@]+\.[^\s@]+$";
            return Regex.IsMatch(email, pattern);
        }
    }
}

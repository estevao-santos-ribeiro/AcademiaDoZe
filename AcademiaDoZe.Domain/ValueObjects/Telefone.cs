// Estevão Santos Ribeiro

using AcademiaDoZe.Domain.Common;
using AcademiaDoZe.Domain.Services;

namespace AcademiaDoZe.Domain.ValueObjects
{
    public sealed record Telefone
    {
        private Telefone(string ddd, string numero)
        {
            Ddd = ddd;
            Numero = numero;
        }

        public string Ddd { get; }

        public string Numero { get; }

        public static Result<Telefone> Criar(string? telefone)
        {
            var notifications = new List<Notification>();

            var normalizado = Normalizer.NormalizeTelefone(telefone);

            if (string.IsNullOrWhiteSpace(normalizado))
            {
                notifications.Add(Notification.Create(nameof(Telefone), "Telefone é obrigatório."));
            }
            else if (normalizado.Length != 10 && normalizado.Length != 11)
            {
                notifications.Add(Notification.Create(nameof(Telefone), "Telefone deve conter 10 ou 11 dígitos."));
            }

            if (notifications.Any())
            {
                return Result<Telefone>.Failure(notifications);
            }

            var ddd = normalizado.Substring(0, 2);
            var numero = normalizado.Substring(2);

            return Result<Telefone>.Success(new Telefone(ddd, numero));
        }

        public string ObterTelefoneSemFormatacao()
            => Ddd + Numero;
    }
}

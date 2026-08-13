// Estevão Santos Ribeiro

using AcademiaDoZe.Domain.Common;

namespace AcademiaDoZe.Domain.ValueObjects
{
    public sealed record Senha
    {
        private Senha(string hash)
        {
            Hash = hash;
        }

        public string Hash { get; }

        public static Result<Senha> Criar(string? hash)
        {
            var notifications = new List<Notification>();

            if (string.IsNullOrWhiteSpace(hash))
            {
                notifications.Add(Notification.Create(nameof(Senha), "Hash de senha é obrigatório."));
            }

            if (notifications.Any())
            {
                return Result<Senha>.Failure(notifications);
            }

            return Result<Senha>.Success(new Senha(hash!));
        }
    }
}

// Estevão Santos Ribeiro

using AcademiaDoZe.Domain.Common;
using AcademiaDoZe.Domain.Services;

namespace AcademiaDoZe.Domain.ValueObjects
{
    public sealed record Arquivo
    {
        private Arquivo(string nome, byte[] conteudo, string contentType, long tamanho)
        {
            Nome = nome;
            Conteudo = conteudo;
            ContentType = contentType;
            Tamanho = tamanho;
        }

        public string Nome { get; }

        public byte[] Conteudo { get; }

        public string ContentType { get; }

        public long Tamanho { get; }

        public static Result<Arquivo> Criar(string? nome, byte[]? conteudo, string? contentType)
        {
            var notifications = new List<Notification>();

            var nomeNormalizado = Normalizer.NormalizeString(nome);

            if (string.IsNullOrWhiteSpace(nomeNormalizado))
            {
                notifications.Add(Notification.Create(nameof(Arquivo), "Nome do arquivo é obrigatório."));
            }

            if (conteudo == null || conteudo.Length == 0)
            {
                notifications.Add(Notification.Create(nameof(Arquivo), "Conteúdo do arquivo é obrigatório."));
            }

            if (string.IsNullOrWhiteSpace(contentType))
            {
                notifications.Add(Notification.Create(nameof(Arquivo), "Tipo de conteúdo do arquivo é obrigatório."));
            }

            if (notifications.Any())
            {
                return Result<Arquivo>.Failure(notifications);
            }

            var tamanho = conteudo?.Length ?? 0;
            return Result<Arquivo>.Success(new Arquivo(nomeNormalizado, conteudo!, contentType!, tamanho));
        }
    }
}

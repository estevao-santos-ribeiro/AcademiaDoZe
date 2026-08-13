// Estevão Santos Ribeiro

using AcademiaDoZe.Domain.Common;
using AcademiaDoZe.Domain.Services;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Entities
{
    public class Logradouro : Entity
    {
        private Logradouro(
            Guid id,
            string nome,
            string bairro,
            string cidade,
            string estado,
            Cep cep)
            : base(id)
        {
            Nome = nome;
            Bairro = bairro;
            Cidade = cidade;
            Estado = estado;
            Cep = cep;
        }

        public string Nome { get; private set; }

        public string Bairro { get; private set; }

        public string Cidade { get; private set; }

        public string Estado { get; private set; }

        public Cep Cep { get; private set; }

        public static Result<Logradouro> Criar(
            string? nome,
            string? bairro,
            string? cidade,
            string? estado,
            string? cep)
        {
            return Criar(Guid.NewGuid(), nome, bairro, cidade, estado, cep);
        }

        public static Result<Logradouro> Criar(
            Guid id,
            string? nome,
            string? bairro,
            string? cidade,
            string? estado,
            string? cep)
        {
            var notifications = new List<Notification>();

            var nomeNormalizado = Normalizer.NormalizeString(nome);
            var bairroNormalizado = Normalizer.NormalizeString(bairro);
            var cidadeNormalizado = Normalizer.NormalizeString(cidade);
            var estadoNormalizado = Normalizer.NormalizeUpperCase(estado);

            if (string.IsNullOrWhiteSpace(nomeNormalizado))
            {
                notifications.Add(Notification.Create(nameof(Nome), "Nome do logradouro é obrigatório."));
            }

            if (string.IsNullOrWhiteSpace(bairroNormalizado))
            {
                notifications.Add(Notification.Create(nameof(Bairro), "Bairro é obrigatório."));
            }

            if (string.IsNullOrWhiteSpace(cidadeNormalizado))
            {
                notifications.Add(Notification.Create(nameof(Cidade), "Cidade é obrigatória."));
            }

            if (string.IsNullOrWhiteSpace(estadoNormalizado))
            {
                notifications.Add(Notification.Create(nameof(Estado), "Estado/UF é obrigatório."));
            }
            else if (estadoNormalizado.Length != 2)
            {
                notifications.Add(Notification.Create(nameof(Estado), "Estado/UF deve ter exatamente 2 caracteres."));
            }

            var resultCep = Cep.Criar(cep);
            if (!resultCep.IsSuccess)
            {
                notifications.AddRange(resultCep.Notifications);
            }

            if (notifications.Any())
            {
                return Result<Logradouro>.Failure(notifications);
            }

            var logradouro = new Logradouro(
                id,
                nomeNormalizado,
                bairroNormalizado,
                cidadeNormalizado,
                estadoNormalizado,
                resultCep.Value!);

            return Result<Logradouro>.Success(logradouro);
        }
    }
}

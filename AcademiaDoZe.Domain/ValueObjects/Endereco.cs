// Estevão Santos Ribeiro

using AcademiaDoZe.Domain.Common;
using AcademiaDoZe.Domain.Services;

namespace AcademiaDoZe.Domain.ValueObjects
{
    public sealed record Endereco
    {
        private Endereco(Guid? logradouroId, string numero, string complemento, Cep cep)
        {
            LogradouroId = logradouroId;
            Numero = numero;
            Complemento = complemento;
            Cep = cep;
        }

        public Guid? LogradouroId { get; }

        public string Numero { get; }

        public string Complemento { get; }

        public Cep Cep { get; }

        public static Result<Endereco> Criar(Guid? logradouroId, string? numero, string? complemento, string? cep)
        {
            var notifications = new List<Notification>();

            var numeroNormalizado = Normalizer.NormalizeString(numero);

            var complementoNormalizado = Normalizer.NormalizeString(complemento ?? string.Empty);

            if (string.IsNullOrWhiteSpace(numeroNormalizado))
            {
                notifications.Add(Notification.Create(nameof(Numero), "Número do endereço é obrigatório."));
            }

            var resultCep = Cep.Criar(cep);
            if (!resultCep.IsSuccess)
            {
                notifications.AddRange(resultCep.Notifications);
            }

            if (notifications.Any())
            {
                return Result<Endereco>.Failure(notifications);
            }

            return Result<Endereco>.Success(
                new Endereco(logradouroId, numeroNormalizado, complementoNormalizado, resultCep.Value!));
        }
    }
}

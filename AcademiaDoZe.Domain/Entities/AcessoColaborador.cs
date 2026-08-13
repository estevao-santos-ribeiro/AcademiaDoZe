// Estevão Santos Ribeiro

using AcademiaDoZe.Domain.Common;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Entities
{

    public class AcessoColaborador : Entity
    {
        private AcessoColaborador(
            Guid id,
            Guid colaboradorId,
            Senha senha,
            bool ativo,
            DateTime? ultimoAcesso)
            : base(id)
        {
            ColaboradorId = colaboradorId;
            Senha = senha;
            Ativo = ativo;
            UltimoAcesso = ultimoAcesso;
        }

        public Guid ColaboradorId { get; private set; }

        public Colaborador? Colaborador { get; private set; }

        public Senha Senha { get; private set; }

        public bool Ativo { get; private set; }

        public DateTime? UltimoAcesso { get; private set; }

        public static Result<AcessoColaborador> Criar(
            Guid colaboradorId,
            string? senhaHash,
            bool ativo,
            DateTime? ultimoAcesso = null)
        {
            return Criar(
                Guid.NewGuid(),
                colaboradorId,
                senhaHash,
                ativo,
                ultimoAcesso);
        }

        public static Result<AcessoColaborador> Criar(
            Guid id,
            Guid colaboradorId,
            string? senhaHash,
            bool ativo,
            DateTime? ultimoAcesso = null)
        {
            var notifications = new List<Notification>();

            if (colaboradorId == Guid.Empty)
            {
                notifications.Add(Notification.Create(nameof(ColaboradorId), "ID do colaborador é obrigatório."));
            }

            var resultSenha = Senha.Criar(senhaHash);
            if (!resultSenha.IsSuccess)
            {
                notifications.AddRange(resultSenha.Notifications);
            }

            if (ultimoAcesso.HasValue && ultimoAcesso.Value > DateTime.UtcNow)
            {
                notifications.Add(Notification.Create(nameof(UltimoAcesso), "Data do último acesso não pode ser no futuro."));
            }

            if (notifications.Any())
            {
                return Result<AcessoColaborador>.Failure(notifications);
            }

            var acesso = new AcessoColaborador(
                id,
                colaboradorId,
                resultSenha.Value!,
                ativo,
                ultimoAcesso);

            return Result<AcessoColaborador>.Success(acesso);
        }

        public void RegistrarAcesso()
        {
            UltimoAcesso = DateTime.UtcNow;
        }

        public void DefinirColaborador(Colaborador colaborador)
        {
            if (colaborador == null)
                throw new ArgumentNullException(nameof(colaborador));

            if (colaborador.Id != ColaboradorId)
                throw new InvalidOperationException("O ID do colaborador não corresponde ao ColaboradorId do acesso.");

            Colaborador = colaborador;
        }
    }
}

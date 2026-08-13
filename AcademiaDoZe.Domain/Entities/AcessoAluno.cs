// Estevão Santos Ribeiro

using AcademiaDoZe.Domain.Common;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Entities
{
    public class AcessoAluno : Entity
    {
        private AcessoAluno(
            Guid id,
            Guid alunoId,
            Senha senha,
            bool ativo,
            DateTime? ultimoAcesso)
            : base(id)
        {
            AlunoId = alunoId;
            Senha = senha;
            Ativo = ativo;
            UltimoAcesso = ultimoAcesso;
        }

        public Guid AlunoId { get; private set; }

        public Aluno? Aluno { get; private set; }

        public Senha Senha { get; private set; }

        public bool Ativo { get; private set; }

        public DateTime? UltimoAcesso { get; private set; }

        public static Result<AcessoAluno> Criar(
            Guid alunoId,
            string? senhaHash,
            bool ativo,
            DateTime? ultimoAcesso = null)
        {
            return Criar(
                Guid.NewGuid(),
                alunoId,
                senhaHash,
                ativo,
                ultimoAcesso);
        }

        public static Result<AcessoAluno> Criar(
            Guid id,
            Guid alunoId,
            string? senhaHash,
            bool ativo,
            DateTime? ultimoAcesso = null)
        {
            var notifications = new List<Notification>();

            if (alunoId == Guid.Empty)
            {
                notifications.Add(Notification.Create(nameof(AlunoId), "ID do aluno é obrigatório."));
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
                return Result<AcessoAluno>.Failure(notifications);
            }

            var acesso = new AcessoAluno(
                id,
                alunoId,
                resultSenha.Value!,
                ativo,
                ultimoAcesso);

            return Result<AcessoAluno>.Success(acesso);
        }

        public void RegistrarAcesso()
        {
            UltimoAcesso = DateTime.UtcNow;
        }

        public void DefinirAluno(Aluno aluno)
        {
            if (aluno == null)
                throw new ArgumentNullException(nameof(aluno));

            if (aluno.Id != AlunoId)
                throw new InvalidOperationException("O ID do aluno não corresponde ao AlunoId do acesso.");

            Aluno = aluno;
        }
    }
}

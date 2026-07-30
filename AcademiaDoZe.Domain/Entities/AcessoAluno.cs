// Estevão Santos Ribeiro
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Entities
{
    public class AcessoAluno : Entity
    {
        public Guid AlunoId { get; private set; }

        public Aluno? Aluno { get; private set; }

        public Senha Senha { get; private set; }

        public DateTime? UltimoAcesso { get; private set; }

        public bool Ativo { get; private set; }

        public AcessoAluno(Guid id, Guid alunoId, Senha senha, bool ativo, DateTime? ultimoAcesso = null)
            : base(id)
        {
            AlunoId = alunoId;
            Senha = senha;
            Ativo = ativo;
            UltimoAcesso = ultimoAcesso;
        }
    }
}

// Estevão Santos Ribeiro
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Entities
{
    public class AcessoColaborador : Entity
    {
        public Guid ColaboradorId { get; private set; }

        public Colaborador? Colaborador { get; private set; }

        public Senha Senha { get; private set; }

        public DateTime? UltimoAcesso { get; private set; }

        public bool Ativo { get; private set; }

        public AcessoColaborador(Guid id, Guid colaboradorId, Senha senha, bool ativo, DateTime? ultimoAcesso = null)
            : base(id)
        {
            ColaboradorId = colaboradorId;
            Senha = senha;
            Ativo = ativo;
            UltimoAcesso = ultimoAcesso;
        }
    }
}

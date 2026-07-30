// Estevão Santos Ribeiro
using AcademiaDoZe.Domain.Enums;

namespace AcademiaDoZe.Domain.Entities
{
    public class Matricula : Entity
    {
        public Guid AlunoId { get; private set; }

        public Aluno? Aluno { get; private set; }

        public MatriculaPlano Plano { get; private set; }

        public MatriculaRestricoes Restricoes { get; private set; }

        public DateTime DataInicio { get; private set; }

        public DateTime? DataFim { get; private set; }

        public decimal Valor { get; private set; }

        public Matricula(Guid id, Guid alunoId, MatriculaPlano plano, MatriculaRestricoes restricoes, DateTime dataInicio, DateTime? dataFim, decimal valor)
            : base(id)
        {
            AlunoId = alunoId;
            Plano = plano;
            Restricoes = restricoes;
            DataInicio = dataInicio;
            DataFim = dataFim;
            Valor = valor;
        }
    }
}

// Estevão Santos Ribeiro

using AcademiaDoZe.Domain.Common;
using AcademiaDoZe.Domain.Enums;

namespace AcademiaDoZe.Domain.Entities
{
    public class Matricula : Entity
    {
        private Matricula(
            Guid id,
            Guid alunoId,
            MatriculaPlano plano,
            MatriculaRestricoes restricoes,
            DateTime dataInicio,
            DateTime? dataFim,
            decimal valor)
            : base(id)
        {
            AlunoId = alunoId;
            Plano = plano;
            Restricoes = restricoes;
            DataInicio = dataInicio;
            DataFim = dataFim;
            Valor = valor;
        }

        public Guid AlunoId { get; private set; }

        public Aluno? Aluno { get; private set; }

        public MatriculaPlano Plano { get; private set; }

        public MatriculaRestricoes Restricoes { get; private set; }

        public DateTime DataInicio { get; private set; }

        public DateTime? DataFim { get; private set; }

        public decimal Valor { get; private set; }

        public static Result<Matricula> Criar(
            Guid alunoId,
            MatriculaPlano plano,
            MatriculaRestricoes restricoes,
            DateTime dataInicio,
            DateTime? dataFim,
            decimal valor)
        {
            return Criar(
                Guid.NewGuid(),
                alunoId,
                plano,
                restricoes,
                dataInicio,
                dataFim,
                valor);
        }

        public static Result<Matricula> Criar(
            Guid id,
            Guid alunoId,
            MatriculaPlano plano,
            MatriculaRestricoes restricoes,
            DateTime dataInicio,
            DateTime? dataFim,
            decimal valor)
        {
            var notifications = new List<Notification>();

            if (alunoId == Guid.Empty)
            {
                notifications.Add(Notification.Create(nameof(AlunoId), "ID do aluno é obrigatório."));
            }

            if (!Enum.IsDefined(typeof(MatriculaPlano), plano))
            {
                notifications.Add(Notification.Create(nameof(Plano), "Plano de matrícula inválido."));
            }

            if (dataInicio == default)
            {
                notifications.Add(Notification.Create(nameof(DataInicio), "Data de início é obrigatória."));
            }

            if (dataInicio != default && dataFim.HasValue && dataInicio > dataFim.Value)
            {
                notifications.Add(Notification.Create(nameof(DataFim), "Data de término não pode ser anterior à data de início."));
            }

            if (valor < 0)
            {
                notifications.Add(Notification.Create(nameof(Valor), "Valor não pode ser negativo."));
            }

            if (notifications.Any())
            {
                return Result<Matricula>.Failure(notifications);
            }

            var matricula = new Matricula(
                id,
                alunoId,
                plano,
                restricoes,
                dataInicio,
                dataFim,
                valor);

            return Result<Matricula>.Success(matricula);
        }

        public void DefinirAluno(Aluno aluno)
        {
            if (aluno == null)
                throw new ArgumentNullException(nameof(aluno));

            if (aluno.Id != AlunoId)
                throw new InvalidOperationException("O ID do aluno não corresponde ao AlunoId da matrícula.");

            Aluno = aluno;
        }
    }
}

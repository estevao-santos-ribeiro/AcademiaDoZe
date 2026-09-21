// Estevão Santos Ribeiro
namespace AcademiaDoZe.Domain.Entities;

public class AcessoAluno : Entity
{
    public Aluno Aluno { get; private set; }
    public DateTime DataHoraChegada { get; private set; }
    public DateTime? DataHoraSaida { get; private set; }

    private AcessoAluno(
        int id,
        Aluno aluno,
        DateTime dataHoraChegada,
        DateTime? dataHoraSaida)
        : base(id)
    {
        Aluno = aluno;
        DataHoraChegada = dataHoraChegada;
        DataHoraSaida = dataHoraSaida;
    }
}

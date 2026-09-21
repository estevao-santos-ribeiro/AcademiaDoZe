// Estevão Santos Ribeiro
namespace AcademiaDoZe.Domain.Entities;

public class AcessoColaborador : Entity
{
    public Colaborador Colaborador { get; private set; }
    public DateTime DataHoraChegada { get; private set; }
    public DateTime? DataHoraSaida { get; private set; }

    private AcessoColaborador(
        int id,
        Colaborador colaborador,
        DateTime dataHoraChegada,
        DateTime? dataHoraSaida)
        : base(id)
    {
        Colaborador = colaborador;
        DataHoraChegada = dataHoraChegada;
        DataHoraSaida = dataHoraSaida;
    }
}
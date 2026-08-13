// Estevão Santos Ribeiro

namespace AcademiaDoZe.Domain.Enums
{
    [Flags]
    public enum MatriculaRestricoes
    {
        Nenhuma = 0,
        Piscina = 1 << 0,
        Musculacao = 1 << 1,
        AulasColetivas = 1 << 2,
        Estacionamento = 1 << 3,
        Spinning = 1 << 4
    }
}

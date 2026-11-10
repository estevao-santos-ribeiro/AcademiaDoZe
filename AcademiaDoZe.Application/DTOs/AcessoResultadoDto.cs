namespace AcademiaDoZe.Application.DTOs;

public class AcessoResultadoDto
{
    public bool Sucesso { get; set; }
    public string Mensagem { get; set; } = string.Empty;
    public bool EstaDentro { get; set; }
    public int? DiasRestantesPlano { get; set; }
    public TimeSpan? TempoPermanencia { get; set; }
    public TimeSpan? TempoTotalDia { get; set; }
}


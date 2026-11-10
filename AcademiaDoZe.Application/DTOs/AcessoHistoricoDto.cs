namespace AcademiaDoZe.Application.DTOs;

public class AcessoHistoricoDto
{
    public int Id { get; set; }
    public int PessoaId { get; set; }
    public int PessoaTipo { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public DateTime DataHora { get; set; }
    public string TipoAcesso { get; set; } = string.Empty;
    public string Detalhes { get; set; } = string.Empty;
}


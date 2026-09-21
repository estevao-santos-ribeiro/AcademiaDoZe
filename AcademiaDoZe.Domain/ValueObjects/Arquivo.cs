// Estevão Santos Ribeiro
namespace AcademiaDoZe.Domain.ValueObjects;

public record Arquivo
{
    public string Nome { get; }
    public string Extensao { get; }
    public byte[] Conteudo { get; }

    private Arquivo(string nome, string extensao, byte[] conteudo)
    {
        Nome = nome;
        Extensao = extensao;
        Conteudo = conteudo;
    }

    public static Arquivo Criar(string nome, string extensao, byte[] conteudo)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new Exception("ARQUIVO_INVALIDO");

        if (string.IsNullOrWhiteSpace(extensao))
            throw new Exception("ARQUIVO_INVALIDO");

        if (conteudo is null || conteudo.Length == 0)
            throw new Exception("ARQUIVO_INVALIDO");

        return new Arquivo(nome, extensao, conteudo);
    }
}

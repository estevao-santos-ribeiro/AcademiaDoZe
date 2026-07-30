// Estevão Santos Ribeiro

namespace AcademiaDoZe.Domain.ValueObjects
{
    public record Arquivo(string Nome, byte[] Conteudo, string ContentType, long Tamanho);
}

using System;

namespace AcademiaDoZe.Domain.ValueObjects
{
    public record Endereco(Guid? LogradouroId, string Numero, string Complemento, Cep Cep);
}

using System;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Entities
{
    public class Logradouro : Entity
    {
        public string Nome { get; private set; }

        public string Bairro { get; private set; }

        public string Cidade { get; private set; }

        public string Estado { get; private set; }

        public Cep Cep { get; private set; }

        public Logradouro(Guid id, string nome, string bairro, string cidade, string estado, Cep cep)
            : base(id)
        {
            Nome = nome;
            Bairro = bairro;
            Cidade = cidade;
            Estado = estado;
            Cep = cep;
        }
    }
}

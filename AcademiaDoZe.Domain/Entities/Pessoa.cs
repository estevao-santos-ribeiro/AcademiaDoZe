using System;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Entities
{
    public class Pessoa : Entity
    {
        public string Nome { get; private set; }

        public DateTime DataNascimento { get; private set; }

        public Cpf Cpf { get; private set; }

        public Email Email { get; private set; }

        public Telefone Telefone { get; private set; }

        public Endereco Endereco { get; private set; }

        public Arquivo? Foto { get; private set; }

        public Pessoa(Guid id, string nome, DateTime dataNascimento, Cpf cpf, Email email, Telefone telefone, Endereco endereco, Arquivo? foto = null)
            : base(id)
        {
            Nome = nome;
            DataNascimento = dataNascimento;
            Cpf = cpf;
            Email = email;
            Telefone = telefone;
            Endereco = endereco;
            Foto = foto;
        }
    }
}

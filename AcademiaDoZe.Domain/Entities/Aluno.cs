// Estevão Santos Ribeiro
using System.Collections.Generic;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Entities
{
    public class Aluno : Pessoa
    {
        public DateTime DataIngresso { get; private set; }

        public string NumeroMatricula { get; private set; }

        private readonly List<Matricula> _matriculas = new();
        public IReadOnlyCollection<Matricula> Matriculas => _matriculas.AsReadOnly();

        public Aluno(Guid id, string nome, DateTime dataNascimento, Cpf cpf, Email email, Telefone telefone, Endereco endereco, DateTime dataIngresso, string numeroMatricula, Arquivo? foto = null)
            : base(id, nome, dataNascimento, cpf, email, telefone, endereco, foto)
        {
            DataIngresso = dataIngresso;
            NumeroMatricula = numeroMatricula;
        }
    }
}

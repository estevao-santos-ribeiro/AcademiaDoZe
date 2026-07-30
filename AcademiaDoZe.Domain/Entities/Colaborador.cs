using System;
using AcademiaDoZe.Domain.ValueObjects;
using AcademiaDoZe.Domain.Enums;

namespace AcademiaDoZe.Domain.Entities
{
    public class Colaborador : Pessoa
    {
        public ColaboradorTipo Tipo { get; private set; }

        public ColaboradorVinculo Vinculo { get; private set; }

        public DateTime DataAdmissao { get; private set; }

        public string? Registro { get; private set; }

        public Colaborador(Guid id, string nome, DateTime dataNascimento, Cpf cpf, Email email, Telefone telefone, Endereco endereco, ColaboradorTipo tipo, ColaboradorVinculo vinculo, DateTime dataAdmissao, string? registro = null, Arquivo? foto = null)
            : base(id, nome, dataNascimento, cpf, email, telefone, endereco, foto)
        {
            Tipo = tipo;
            Vinculo = vinculo;
            DataAdmissao = dataAdmissao;
            Registro = registro;
        }
    }
}

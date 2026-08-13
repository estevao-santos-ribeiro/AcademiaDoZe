// Estevão Santos Ribeiro

using System.Collections.Generic;
using AcademiaDoZe.Domain.Common;
using AcademiaDoZe.Domain.Services;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Entities
{
    public class Aluno : Pessoa
    {
        private readonly List<Matricula> _matriculas = new();

        private Aluno(
            Guid id,
            string nome,
            DateTime dataNascimento,
            Cpf cpf,
            Email email,
            Telefone telefone,
            Endereco endereco,
            DateTime dataIngresso,
            string numeroMatricula,
            Arquivo? foto)
            : base(id, nome, dataNascimento, cpf, email, telefone, endereco, foto)
        {
            DataIngresso = dataIngresso;
            NumeroMatricula = numeroMatricula;
        }

        public DateTime DataIngresso { get; private set; }

        public string NumeroMatricula { get; private set; }

        public IReadOnlyCollection<Matricula> Matriculas => _matriculas.AsReadOnly();

        public static Result<Aluno> Criar(
            string? nome,
            DateTime dataNascimento,
            string? cpf,
            string? email,
            string? telefoneDdd,
            string? telefoneNumero,
            Guid? logradouroId,
            string? enderecoCep,
            string? enderecoNumero,
            string? enderecoComplemento,
            DateTime dataIngresso,
            string? numeroMatricula,
            byte[]? fotoConteudo = null,
            string? fotoNome = null,
            string? fotoContentType = null)
        {
            return Criar(
                Guid.NewGuid(),
                nome,
                dataNascimento,
                cpf,
                email,
                telefoneDdd,
                telefoneNumero,
                logradouroId,
                enderecoCep,
                enderecoNumero,
                enderecoComplemento,
                dataIngresso,
                numeroMatricula,
                fotoConteudo,
                fotoNome,
                fotoContentType);
        }

        public static Result<Aluno> Criar(
            Guid id,
            string? nome,
            DateTime dataNascimento,
            string? cpf,
            string? email,
            string? telefoneDdd,
            string? telefoneNumero,
            Guid? logradouroId,
            string? enderecoCep,
            string? enderecoNumero,
            string? enderecoComplemento,
            DateTime dataIngresso,
            string? numeroMatricula,
            byte[]? fotoConteudo = null,
            string? fotoNome = null,
            string? fotoContentType = null)
        {
            var resultPessoa = Pessoa.Criar(
                id,
                nome,
                dataNascimento,
                cpf,
                email,
                telefoneDdd,
                telefoneNumero,
                logradouroId,
                enderecoCep,
                enderecoNumero,
                enderecoComplemento,
                fotoConteudo,
                fotoNome,
                fotoContentType);

            var notifications = new List<Notification>();

            if (!resultPessoa.IsSuccess)
            {
                notifications.AddRange(resultPessoa.Notifications);
            }

            var numMatriculaNormalizado = Normalizer.NormalizeString(numeroMatricula);

            if (dataIngresso == default)
            {
                notifications.Add(Notification.Create(nameof(DataIngresso), "Data de ingresso é obrigatória."));
            }
            else if (dataIngresso > DateTime.Today)
            {
                notifications.Add(Notification.Create(nameof(DataIngresso), "Data de ingresso não pode ser no futuro."));
            }

            if (string.IsNullOrWhiteSpace(numMatriculaNormalizado))
            {
                notifications.Add(Notification.Create(nameof(NumeroMatricula), "Número de matrícula é obrigatório."));
            }

            if (notifications.Any())
            {
                return Result<Aluno>.Failure(notifications);
            }

            var pessoa = resultPessoa.Value!;
            var aluno = new Aluno(
                pessoa.Id,
                pessoa.Nome,
                pessoa.DataNascimento,
                pessoa.Cpf,
                pessoa.Email,
                pessoa.Telefone,
                pessoa.Endereco,
                dataIngresso,
                numMatriculaNormalizado,
                pessoa.Foto);

            return Result<Aluno>.Success(aluno);
        }

        public void AdicionarMatricula(Matricula matricula)
        {
            if (matricula == null)
                throw new ArgumentNullException(nameof(matricula));

            _matriculas.Add(matricula);
        }
    }
}

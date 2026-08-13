// Estevão Santos Ribeiro

using AcademiaDoZe.Domain.Common;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.Services;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Entities
{
    public class Colaborador : Pessoa
    {
        private Colaborador(
            Guid id,
            string nome,
            DateTime dataNascimento,
            Cpf cpf,
            Email email,
            Telefone telefone,
            Endereco endereco,
            ColaboradorTipo tipo,
            ColaboradorVinculo vinculo,
            DateTime dataAdmissao,
            string? registro,
            Arquivo? foto)
            : base(id, nome, dataNascimento, cpf, email, telefone, endereco, foto)
        {
            Tipo = tipo;
            Vinculo = vinculo;
            DataAdmissao = dataAdmissao;
            Registro = registro;
        }

        public ColaboradorTipo Tipo { get; private set; }

        public ColaboradorVinculo Vinculo { get; private set; }

        public DateTime DataAdmissao { get; private set; }

        public string? Registro { get; private set; }

        public static Result<Colaborador> Criar(
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
            ColaboradorTipo tipo,
            ColaboradorVinculo vinculo,
            DateTime dataAdmissao,
            string? registro = null,
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
                tipo,
                vinculo,
                dataAdmissao,
                registro,
                fotoConteudo,
                fotoNome,
                fotoContentType);
        }

        public static Result<Colaborador> Criar(
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
            ColaboradorTipo tipo,
            ColaboradorVinculo vinculo,
            DateTime dataAdmissao,
            string? registro = null,
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

            var registroNormalizado = Normalizer.NormalizeString(registro ?? string.Empty);

            if (!Enum.IsDefined(typeof(ColaboradorTipo), tipo))
            {
                notifications.Add(Notification.Create(nameof(Tipo), "Tipo de colaborador inválido."));
            }

            if (!Enum.IsDefined(typeof(ColaboradorVinculo), vinculo))
            {
                notifications.Add(Notification.Create(nameof(Vinculo), "Vínculo de trabalho inválido."));
            }

            if (dataAdmissao == default)
            {
                notifications.Add(Notification.Create(nameof(DataAdmissao), "Data de admissão é obrigatória."));
            }
            else if (dataAdmissao > DateTime.Today)
            {
                notifications.Add(Notification.Create(nameof(DataAdmissao), "Data de admissão não pode ser no futuro."));
            }

            if (notifications.Any())
            {
                return Result<Colaborador>.Failure(notifications);
            }

            var pessoa = resultPessoa.Value!;
            var colaborador = new Colaborador(
                pessoa.Id,
                pessoa.Nome,
                pessoa.DataNascimento,
                pessoa.Cpf,
                pessoa.Email,
                pessoa.Telefone,
                pessoa.Endereco,
                tipo,
                vinculo,
                dataAdmissao,
                string.IsNullOrWhiteSpace(registroNormalizado) ? null : registroNormalizado,
                pessoa.Foto);

            return Result<Colaborador>.Success(colaborador);
        }
    }
}

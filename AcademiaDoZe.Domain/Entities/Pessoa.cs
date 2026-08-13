// Estevão Santos Ribeiro

using AcademiaDoZe.Domain.Common;
using AcademiaDoZe.Domain.Services;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Entities
{
    public class Pessoa : Entity
    {
        protected Pessoa(Guid id, string nome, DateTime dataNascimento, Cpf cpf, Email email, Telefone telefone, Endereco endereco, Arquivo? foto)
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

        public string Nome { get; protected set; }

        public DateTime DataNascimento { get; protected set; }

        public Cpf Cpf { get; protected set; }

        public Email Email { get; protected set; }

        public Telefone Telefone { get; protected set; }

        public Endereco Endereco { get; protected set; }

        public Arquivo? Foto { get; protected set; }

        public static Result<Pessoa> Criar(
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
                fotoConteudo,
                fotoNome,
                fotoContentType);
        }

        public static Result<Pessoa> Criar(
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
            byte[]? fotoConteudo = null,
            string? fotoNome = null,
            string? fotoContentType = null)
        {
            var notifications = new List<Notification>();

            var nomeNormalizado = Normalizer.NormalizeString(nome);

            if (string.IsNullOrWhiteSpace(nomeNormalizado))
            {
                notifications.Add(Notification.Create(nameof(Nome), "Nome é obrigatório."));
            }

            if (dataNascimento == default)
            {
                notifications.Add(Notification.Create(nameof(DataNascimento), "Data de nascimento é obrigatória."));
            }
            else if (dataNascimento.Year < 1900)
            {
                notifications.Add(Notification.Create(nameof(DataNascimento), "Data de nascimento não pode ser anterior a 1900."));
            }
            else if (dataNascimento > DateTime.Today)
            {
                notifications.Add(Notification.Create(nameof(DataNascimento), "Data de nascimento não pode ser no futuro."));
            }

            var resultCpf = Cpf.Criar(cpf);
            if (!resultCpf.IsSuccess)
            {
                notifications.AddRange(resultCpf.Notifications);
            }

            var resultEmail = Email.Criar(email);
            if (!resultEmail.IsSuccess)
            {
                notifications.AddRange(resultEmail.Notifications);
            }

            var telefoneCombinado = GetTelefoneCombinado(telefoneDdd, telefoneNumero);
            var resultTelefone = Telefone.Criar(telefoneCombinado);
            if (!resultTelefone.IsSuccess)
            {
                notifications.AddRange(resultTelefone.Notifications);
            }

            var resultEndereco = Endereco.Criar(logradouroId, enderecoNumero, enderecoComplemento, enderecoCep);
            if (!resultEndereco.IsSuccess)
            {
                notifications.AddRange(resultEndereco.Notifications);
            }

            Arquivo? foto = null;
            if (fotoConteudo != null && fotoConteudo.Length > 0)
            {
                var resultFoto = Arquivo.Criar(fotoNome, fotoConteudo, fotoContentType);
                if (!resultFoto.IsSuccess)
                {
                    notifications.AddRange(resultFoto.Notifications);
                }
                else
                {
                    foto = resultFoto.Value;
                }
            }

            if (notifications.Any())
            {
                return Result<Pessoa>.Failure(notifications);
            }

            var pessoa = new Pessoa(
                id,
                nomeNormalizado,
                dataNascimento,
                resultCpf.Value!,
                resultEmail.Value!,
                resultTelefone.Value!,
                resultEndereco.Value!,
                foto);

            return Result<Pessoa>.Success(pessoa);
        }

        private static string GetTelefoneCombinado(string? ddd, string? numero)
        {
            if (!string.IsNullOrWhiteSpace(ddd) && !string.IsNullOrWhiteSpace(numero))
                return ddd + numero;

            if (!string.IsNullOrWhiteSpace(ddd))
                return ddd;

            return numero ?? string.Empty;
        }
    }
}

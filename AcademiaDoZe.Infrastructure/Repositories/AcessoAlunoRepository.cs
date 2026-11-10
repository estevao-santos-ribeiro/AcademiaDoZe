using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Repositories;
using AcademiaDoZe.Infrastructure.Data;
using AcademiaDoZe.Infrastructure.Exceptions;
using System.Data;
using System.Data.Common;

namespace AcademiaDoZe.Infrastructure.Repositories;

public class AcessoAlunoRepository : BaseRepository, IAcessoAlunoRepository
{
    public AcessoAlunoRepository(string connectionString, DatabaseType databaseType) : base(connectionString, databaseType)
    {
    }

    private static AcessoAluno Map(DbDataReader reader)
    {
        int id = reader.GetInt32Value("id_acesso");
        int alunoId = reader.GetInt32Value("pessoa_id");
        DateTime dataHora = reader.GetDateTimeValue("data_hora");

        var result = AcessoAluno.Criar(id, alunoId, dataHora);
        if (result.IsFailure)
        {
            throw new InfrastructureException("ERRO_MAPEAMENTO_ACESSO_ALUNO", $"Erro ao mapear acesso aluno ID {id}: {string.Join(", ", result.Notifications)}");
        }

        return result.Value!;
    }

    public async Task<AcessoAluno?> ObterPorId(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            const string query = "SELECT id_acesso, pessoa_tipo, pessoa_id, data_hora FROM tb_acesso WHERE id_acesso = @Id AND pessoa_tipo = 0";
            using var cmd = await CreateCommandAsync(query, cancellationToken);
            cmd.AddParameter("@Id", id, DbType.Int32);

            using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
            if (await reader.ReadAsync(cancellationToken))
            {
                return Map(reader);
            }

            return null;
        }
        catch (DbException ex)
        {
            throw new InfrastructureException("ERRO_OBTER_ACESSO_ALUNO_POR_ID", $"Erro ao obter acesso aluno ID {id}: {ex.Message}", ex);
        }
    }

    public async Task<IEnumerable<AcessoAluno>> ObterTodos(CancellationToken cancellationToken = default)
    {
        try
        {
            const string query = "SELECT id_acesso, pessoa_tipo, pessoa_id, data_hora FROM tb_acesso WHERE pessoa_tipo = 0 ORDER BY data_hora DESC";
            using var cmd = await CreateCommandAsync(query, cancellationToken);
            using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

            var list = new List<AcessoAluno>();
            while (await reader.ReadAsync(cancellationToken))
            {
                list.Add(Map(reader));
            }

            return list;
        }
        catch (DbException ex)
        {
            throw new InfrastructureException("ERRO_OBTER_TODOS_ACESSOS_ALUNO", $"Erro ao obter todos os acessos de aluno: {ex.Message}", ex);
        }
    }

    public async Task<AcessoAluno> Adicionar(AcessoAluno entity, CancellationToken cancellationToken = default)
    {
        try
        {
            string query = FormatInsertQuery("INSERT INTO tb_acesso (pessoa_tipo, pessoa_id, data_hora) VALUES (@PessoaTipo, @PessoaId, @DataHora)");
            using var cmd = await CreateCommandAsync(query, cancellationToken);
            cmd.AddParameter("@PessoaTipo", 0, DbType.Int32);
            cmd.AddParameter("@PessoaId", entity.AlunoId, DbType.Int32);
            cmd.AddParameter("@DataHora", entity.DataHora.ToString("yyyy-MM-dd HH:mm:ss"), DbType.String);

            var newId = Convert.ToInt32(await cmd.ExecuteScalarAsync(cancellationToken));
            var result = AcessoAluno.Criar(newId, entity.AlunoId, entity.DataHora);
            return result.Value!;
        }
        catch (DbException ex)
        {
            throw new InfrastructureException("ERRO_ADICIONAR_ACESSO_ALUNO", $"Erro ao registrar acesso do aluno {entity.AlunoId}: {ex.Message}", ex);
        }
    }

    public async Task<AcessoAluno> Atualizar(AcessoAluno entity, CancellationToken cancellationToken = default)
    {
        try
        {
            const string query = "UPDATE tb_acesso SET data_hora = @DataHora WHERE id_acesso = @Id AND pessoa_tipo = 0";
            using var cmd = await CreateCommandAsync(query, cancellationToken);
            cmd.AddParameter("@Id", entity.Id, DbType.Int32);
            cmd.AddParameter("@DataHora", entity.DataHora.ToString("yyyy-MM-dd HH:mm:ss"), DbType.String);

            await cmd.ExecuteNonQueryAsync(cancellationToken);
            return entity;
        }
        catch (DbException ex)
        {
            throw new InfrastructureException("ERRO_ATUALIZAR_ACESSO_ALUNO", $"Erro ao atualizar acesso do aluno {entity.Id}: {ex.Message}", ex);
        }
    }

    public async Task<bool> Remover(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            const string query = "DELETE FROM tb_acesso WHERE id_acesso = @Id AND pessoa_tipo = 0";
            using var cmd = await CreateCommandAsync(query, cancellationToken);
            cmd.AddParameter("@Id", id, DbType.Int32);

            int rows = await cmd.ExecuteNonQueryAsync(cancellationToken);
            return rows > 0;
        }
        catch (DbException ex)
        {
            throw new InfrastructureException("ERRO_REMOVER_ACESSO_ALUNO", $"Erro ao remover acesso aluno ID {id}: {ex.Message}", ex);
        }
    }

    public async Task<IEnumerable<AcessoAluno>> ObterAcessosPorAlunoPeriodo(
        int? alunoId = null,
        DateOnly? inicio = null,
        DateOnly? fim = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = "SELECT id_acesso, pessoa_tipo, pessoa_id, data_hora FROM tb_acesso WHERE pessoa_tipo = 0";
            if (alunoId.HasValue && alunoId.Value > 0)
            {
                query += " AND pessoa_id = @AlunoId";
            }

            if (inicio.HasValue)
            {
                query += " AND data_hora >= @Inicio";
            }

            if (fim.HasValue)
            {
                query += " AND data_hora <= @Fim";
            }

            query += " ORDER BY data_hora DESC";

            using var cmd = await CreateCommandAsync(query, cancellationToken);
            if (alunoId.HasValue && alunoId.Value > 0)
            {
                cmd.AddParameter("@AlunoId", alunoId.Value, DbType.Int32);
            }

            if (inicio.HasValue)
            {
                cmd.AddParameter("@Inicio", inicio.Value.ToString("yyyy-MM-dd 00:00:00"), DbType.String);
            }

            if (fim.HasValue)
            {
                cmd.AddParameter("@Fim", fim.Value.ToString("yyyy-MM-dd 23:59:59"), DbType.String);
            }

            using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
            var list = new List<AcessoAluno>();
            while (await reader.ReadAsync(cancellationToken))
            {
                list.Add(Map(reader));
            }

            return list;
        }
        catch (DbException ex)
        {
            throw new InfrastructureException("ERRO_OBTER_ACESSO_ALUNO_PERIODO", $"Erro ao obter acessos de aluno no período: {ex.Message}", ex);
        }
    }

    public async Task<AcessoAluno?> ObterUltimoAcesso(int alunoId, CancellationToken cancellationToken = default)
    {
        try
        {
            const string query = "SELECT id_acesso, pessoa_tipo, pessoa_id, data_hora FROM tb_acesso WHERE pessoa_tipo = 0 AND pessoa_id = @AlunoId ORDER BY data_hora DESC, id_acesso DESC LIMIT 1";
            using var cmd = await CreateCommandAsync(query, cancellationToken);
            cmd.AddParameter("@AlunoId", alunoId, DbType.Int32);

            using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
            if (await reader.ReadAsync(cancellationToken))
            {
                return Map(reader);
            }

            return null;
        }
        catch (DbException ex)
        {
            throw new InfrastructureException("ERRO_OBTER_ULTIMO_ACESSO_ALUNO", $"Erro ao obter último acesso do aluno {alunoId}: {ex.Message}", ex);
        }
    }

    public async Task<bool> EstaNaAcademia(int alunoId, CancellationToken cancellationToken = default)
    {
        try
        {
            var hoje = DateTime.Today.ToString("yyyy-MM-dd");
            const string query = "SELECT COUNT(*) FROM tb_acesso WHERE pessoa_tipo = 0 AND pessoa_id = @AlunoId AND data_hora >= @HojeInicio";
            using var cmd = await CreateCommandAsync(query, cancellationToken);
            cmd.AddParameter("@AlunoId", alunoId, DbType.Int32);
            cmd.AddParameter("@HojeInicio", $"{hoje} 00:00:00", DbType.String);

            var count = Convert.ToInt32(await cmd.ExecuteScalarAsync(cancellationToken));
            return count % 2 != 0;
        }
        catch (DbException ex)
        {
            throw new InfrastructureException("ERRO_VERIFICAR_ESTA_NA_ACADEMIA", $"Erro ao verificar se aluno {alunoId} está na academia: {ex.Message}", ex);
        }
    }

    public Task<Dictionary<TimeOnly, int>> ObterHorarioMaisProcuradoPorMes(int mes, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new Dictionary<TimeOnly, int>());
    }

    public Task<Dictionary<int, TimeSpan>> ObterPermanenciaMediaPorMes(int mes, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new Dictionary<int, TimeSpan>());
    }

    public Task<IEnumerable<Aluno>> ObterAlunosSemAcessoNosUltimosDias(int dias, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<Aluno>>([]);
    }
}

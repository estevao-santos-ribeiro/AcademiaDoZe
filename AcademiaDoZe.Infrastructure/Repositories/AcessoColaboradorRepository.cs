using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Repositories;
using AcademiaDoZe.Infrastructure.Data;
using AcademiaDoZe.Infrastructure.Exceptions;
using System.Data;
using System.Data.Common;

namespace AcademiaDoZe.Infrastructure.Repositories;

public class AcessoColaboradorRepository : BaseRepository, IAcessoColaboradorRepository
{
    public AcessoColaboradorRepository(string connectionString, DatabaseType databaseType) : base(connectionString, databaseType)
    {
    }

    private static AcessoColaborador Map(DbDataReader reader)
    {
        int id = reader.GetInt32Value("id_acesso");
        int colaboradorId = reader.GetInt32Value("pessoa_id");
        DateTime dataHora = reader.GetDateTimeValue("data_hora");

        var result = AcessoColaborador.Criar(id, colaboradorId, dataHora);
        if (result.IsFailure)
        {
            throw new InfrastructureException("ERRO_MAPEAMENTO_ACESSO_COLABORADOR", $"Erro ao mapear acesso colaborador ID {id}: {string.Join(", ", result.Notifications)}");
        }

        return result.Value!;
    }

    public async Task<AcessoColaborador?> ObterPorId(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            const string query = "SELECT id_acesso, pessoa_tipo, pessoa_id, data_hora FROM tb_acesso WHERE id_acesso = @Id AND pessoa_tipo = 1";
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
            throw new InfrastructureException("ERRO_OBTER_ACESSO_COLABORADOR_POR_ID", $"Erro ao obter acesso colaborador ID {id}: {ex.Message}", ex);
        }
    }

    public async Task<IEnumerable<AcessoColaborador>> ObterTodos(CancellationToken cancellationToken = default)
    {
        try
        {
            const string query = "SELECT id_acesso, pessoa_tipo, pessoa_id, data_hora FROM tb_acesso WHERE pessoa_tipo = 1 ORDER BY data_hora DESC";
            using var cmd = await CreateCommandAsync(query, cancellationToken);
            using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

            var list = new List<AcessoColaborador>();
            while (await reader.ReadAsync(cancellationToken))
            {
                list.Add(Map(reader));
            }

            return list;
        }
        catch (DbException ex)
        {
            throw new InfrastructureException("ERRO_OBTER_TODOS_ACESSOS_COLABORADOR", $"Erro ao obter todos os acessos de colaborador: {ex.Message}", ex);
        }
    }

    public async Task<AcessoColaborador> Adicionar(AcessoColaborador entity, CancellationToken cancellationToken = default)
    {
        try
        {
            string query = FormatInsertQuery("INSERT INTO tb_acesso (pessoa_tipo, pessoa_id, data_hora) VALUES (@PessoaTipo, @PessoaId, @DataHora)");
            using var cmd = await CreateCommandAsync(query, cancellationToken);
            cmd.AddParameter("@PessoaTipo", 1, DbType.Int32);
            cmd.AddParameter("@PessoaId", entity.ColaboradorId, DbType.Int32);
            cmd.AddParameter("@DataHora", entity.DataHora.ToString("yyyy-MM-dd HH:mm:ss"), DbType.String);

            var newId = Convert.ToInt32(await cmd.ExecuteScalarAsync(cancellationToken));
            var result = AcessoColaborador.Criar(newId, entity.ColaboradorId, entity.DataHora);
            return result.Value!;
        }
        catch (DbException ex)
        {
            throw new InfrastructureException("ERRO_ADICIONAR_ACESSO_COLABORADOR", $"Erro ao registrar acesso do colaborador {entity.ColaboradorId}: {ex.Message}", ex);
        }
    }

    public async Task<AcessoColaborador> Atualizar(AcessoColaborador entity, CancellationToken cancellationToken = default)
    {
        try
        {
            const string query = "UPDATE tb_acesso SET data_hora = @DataHora WHERE id_acesso = @Id AND pessoa_tipo = 1";
            using var cmd = await CreateCommandAsync(query, cancellationToken);
            cmd.AddParameter("@Id", entity.Id, DbType.Int32);
            cmd.AddParameter("@DataHora", entity.DataHora.ToString("yyyy-MM-dd HH:mm:ss"), DbType.String);

            await cmd.ExecuteNonQueryAsync(cancellationToken);
            return entity;
        }
        catch (DbException ex)
        {
            throw new InfrastructureException("ERRO_ATUALIZAR_ACESSO_COLABORADOR", $"Erro ao atualizar acesso do colaborador {entity.Id}: {ex.Message}", ex);
        }
    }

    public async Task<bool> Remover(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            const string query = "DELETE FROM tb_acesso WHERE id_acesso = @Id AND pessoa_tipo = 1";
            using var cmd = await CreateCommandAsync(query, cancellationToken);
            cmd.AddParameter("@Id", id, DbType.Int32);

            int rows = await cmd.ExecuteNonQueryAsync(cancellationToken);
            return rows > 0;
        }
        catch (DbException ex)
        {
            throw new InfrastructureException("ERRO_REMOVER_ACESSO_COLABORADOR", $"Erro ao remover acesso colaborador ID {id}: {ex.Message}", ex);
        }
    }

    public async Task<IEnumerable<AcessoColaborador>> ObterAcessosPorColaboradorPeriodo(
        int? colaboradorId = null,
        DateOnly? inicio = null,
        DateOnly? fim = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = "SELECT id_acesso, pessoa_tipo, pessoa_id, data_hora FROM tb_acesso WHERE pessoa_tipo = 1";
            if (colaboradorId.HasValue && colaboradorId.Value > 0)
            {
                query += " AND pessoa_id = @ColaboradorId";
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
            if (colaboradorId.HasValue && colaboradorId.Value > 0)
            {
                cmd.AddParameter("@ColaboradorId", colaboradorId.Value, DbType.Int32);
            }

            if (inicio.HasValue)
            {
                cmd.AddParameter("@Inicio", $"{inicio.Value:yyyy-MM-dd} 00:00:00", DbType.String);
            }

            if (fim.HasValue)
            {
                cmd.AddParameter("@Fim", $"{fim.Value:yyyy-MM-dd} 23:59:59", DbType.String);
            }

            using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
            var list = new List<AcessoColaborador>();
            while (await reader.ReadAsync(cancellationToken))
            {
                list.Add(Map(reader));
            }

            return list;
        }
        catch (DbException ex)
        {
            throw new InfrastructureException("ERRO_OBTER_ACESSO_COLABORADOR_PERIODO", $"Erro ao obter acessos de colaborador no período: {ex.Message}", ex);
        }
    }

    public async Task<AcessoColaborador?> ObterUltimoAcesso(int colaboradorId, CancellationToken cancellationToken = default)
    {
        try
        {
            const string query = "SELECT id_acesso, pessoa_tipo, pessoa_id, data_hora FROM tb_acesso WHERE pessoa_tipo = 1 AND pessoa_id = @ColaboradorId ORDER BY data_hora DESC, id_acesso DESC LIMIT 1";
            using var cmd = await CreateCommandAsync(query, cancellationToken);
            cmd.AddParameter("@ColaboradorId", colaboradorId, DbType.Int32);

            using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
            if (await reader.ReadAsync(cancellationToken))
            {
                return Map(reader);
            }

            return null;
        }
        catch (DbException ex)
        {
            throw new InfrastructureException("ERRO_OBTER_ULTIMO_ACESSO_COLABORADOR", $"Erro ao obter último acesso do colaborador {colaboradorId}: {ex.Message}", ex);
        }
    }

    public async Task<TimeSpan> ObterHorasTrabalhadasNoDia(int colaboradorId, DateOnly data, CancellationToken cancellationToken = default)
    {
        try
        {
            var acessos = (await ObterAcessosPorColaboradorPeriodo(colaboradorId, data, data, cancellationToken))
                .OrderBy(a => a.DataHora)
                .ToList();

            var total = TimeSpan.Zero;
            for (int i = 0; i + 1 < acessos.Count; i += 2)
            {
                total += (acessos[i + 1].DataHora - acessos[i].DataHora);
            }

            if (acessos.Count % 2 != 0)
            {
                total += (DateTime.Now - acessos[^1].DataHora);
            }

            return total;
        }
        catch (Exception ex)
        {
            throw new InfrastructureException("ERRO_CALCULAR_HORAS_COLABORADOR", $"Erro ao calcular horas do colaborador {colaboradorId}: {ex.Message}", ex);
        }
    }
}

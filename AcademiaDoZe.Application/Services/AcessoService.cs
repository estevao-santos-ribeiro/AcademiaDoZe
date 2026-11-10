using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Enums;
using AcademiaDoZe.Application.Interfaces;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Repositories;

namespace AcademiaDoZe.Application.Services;

public class AcessoService : IAcessoService
{
    private readonly IAcessoAlunoRepository _acessoAlunoRepo;
    private readonly IAcessoColaboradorRepository _acessoColaboradorRepo;
    private readonly IAlunoService _alunoService;
    private readonly IColaboradorService _colaboradorService;
    private readonly IMatriculaService _matriculaService;

    public AcessoService(
        Func<IAcessoAlunoRepository> alunoRepoFactory,
        Func<IAcessoColaboradorRepository> colabRepoFactory,
        IAlunoService alunoService,
        IColaboradorService colaboradorService,
        IMatriculaService matriculaService)
    {
        _acessoAlunoRepo = alunoRepoFactory();
        _acessoColaboradorRepo = colabRepoFactory();
        _alunoService = alunoService;
        _colaboradorService = colaboradorService;
        _matriculaService = matriculaService;
    }

    public async Task<AcessoResultadoDto> VerificarStatusAlunoAsync(int alunoId, CancellationToken cancellationToken = default)
    {
        var aluno = await _alunoService.ObterPorIdAsync(alunoId, cancellationToken);
        if (aluno == null)
        {
            return new AcessoResultadoDto { Sucesso = false, Mensagem = "Aluno não encontrado." };
        }

        bool estaDentro = await _acessoAlunoRepo.EstaNaAcademia(alunoId, cancellationToken);
        var matricula = await _matriculaService.ObterMatriculaAtivaPorAlunoAsync(alunoId, cancellationToken);

        int? diasRestantes = null;
        if (matricula != null)
        {
            diasRestantes = Math.Max(0, (matricula.DataFim.ToDateTime(TimeOnly.MinValue) - DateTime.Today).Days);
        }

        return new AcessoResultadoDto
        {
            Sucesso = true,
            EstaDentro = estaDentro,
            DiasRestantesPlano = diasRestantes
        };
    }

    public async Task<AcessoResultadoDto> RegistrarAcessoAlunoAsync(int alunoId, CancellationToken cancellationToken = default)
    {
        var aluno = await _alunoService.ObterPorIdAsync(alunoId, cancellationToken);
        if (aluno == null)
        {
            return new AcessoResultadoDto { Sucesso = false, Mensagem = "Aluno não encontrado." };
        }

        bool estaDentro = await _acessoAlunoRepo.EstaNaAcademia(alunoId, cancellationToken);

        if (!estaDentro)
        {
            var matricula = await _matriculaService.ObterMatriculaAtivaPorAlunoAsync(alunoId, cancellationToken);
            if (matricula == null)
            {
                return new AcessoResultadoDto
                {
                    Sucesso = false,
                    EstaDentro = false,
                    Mensagem = "Acesso negado: o aluno não possui matrícula ativa"
                };
            }

            int dias = Math.Max(0, (matricula.DataFim.ToDateTime(TimeOnly.MinValue) - DateTime.Today).Days);

            var novoAcesso = AcessoAluno.Criar(0, alunoId, DateTime.Now);
            await _acessoAlunoRepo.Adicionar(novoAcesso.Value!, cancellationToken);

            return new AcessoResultadoDto
            {
                Sucesso = true,
                EstaDentro = true,
                DiasRestantesPlano = dias,
                Mensagem = $"Entrada autorizada. Dias restantes de plano: {dias} dia(s)."
            };
        }
        else
        {
            var ultimoAcesso = await _acessoAlunoRepo.ObterUltimoAcesso(alunoId, cancellationToken);
            var permanencia = ultimoAcesso != null ? (DateTime.Now - ultimoAcesso.DataHora) : TimeSpan.Zero;
            if (permanencia < TimeSpan.Zero) permanencia = TimeSpan.Zero;

            var novoAcesso = AcessoAluno.Criar(0, alunoId, DateTime.Now);
            await _acessoAlunoRepo.Adicionar(novoAcesso.Value!, cancellationToken);

            string tempoStr = permanencia.ToString(@"hh\:mm\:ss");
            return new AcessoResultadoDto
            {
                Sucesso = true,
                EstaDentro = false,
                TempoPermanencia = permanencia,
                Mensagem = $"Saída registrada. Tempo de permanência: {tempoStr}"
            };
        }
    }

    public async Task<AcessoResultadoDto> VerificarStatusColaboradorAsync(int colaboradorId, CancellationToken cancellationToken = default)
    {
        var colab = await _colaboradorService.ObterPorIdAsync(colaboradorId, cancellationToken);
        if (colab == null)
        {
            return new AcessoResultadoDto { Sucesso = false, Mensagem = "Colaborador não encontrado." };
        }

        var hoje = DateOnly.FromDateTime(DateTime.Today);
        var acessosHoje = (await _acessoColaboradorRepo.ObterAcessosPorColaboradorPeriodo(colaboradorId, hoje, hoje, cancellationToken)).ToList();
        bool estaDentro = acessosHoje.Count % 2 != 0;

        var totalHoras = await _acessoColaboradorRepo.ObterHorasTrabalhadasNoDia(colaboradorId, hoje, cancellationToken);

        return new AcessoResultadoDto
        {
            Sucesso = true,
            EstaDentro = estaDentro,
            TempoTotalDia = totalHoras
        };
    }

    public async Task<AcessoResultadoDto> RegistrarAcessoColaboradorAsync(int colaboradorId, CancellationToken cancellationToken = default)
    {
        var colab = await _colaboradorService.ObterPorIdAsync(colaboradorId, cancellationToken);
        if (colab == null)
        {
            return new AcessoResultadoDto { Sucesso = false, Mensagem = "Colaborador não encontrado." };
        }

        var limite = colab.Vinculo == AppColaboradorVinculo.Estagio
            ? TimeSpan.FromHours(6)
            : TimeSpan.FromHours(8);

        var hoje = DateOnly.FromDateTime(DateTime.Today);
        var acessosHoje = (await _acessoColaboradorRepo.ObterAcessosPorColaboradorPeriodo(colaboradorId, hoje, hoje, cancellationToken))
            .OrderBy(a => a.DataHora)
            .ToList();

        bool estaDentro = acessosHoje.Count % 2 != 0;

        if (!estaDentro)
        {
            var totalTrabalhado = await _acessoColaboradorRepo.ObterHorasTrabalhadasNoDia(colaboradorId, hoje, cancellationToken);
            if (totalTrabalhado >= limite)
            {
                return new AcessoResultadoDto
                {
                    Sucesso = false,
                    EstaDentro = false,
                    Mensagem = $"Acesso negado: colaborador já atingiu o limite de {limite.TotalHours} horas diárias ({colab.Vinculo})."
                };
            }

            var novoAcesso = AcessoColaborador.Criar(0, colaboradorId, DateTime.Now);
            await _acessoColaboradorRepo.Adicionar(novoAcesso.Value!, cancellationToken);

            return new AcessoResultadoDto
            {
                Sucesso = true,
                EstaDentro = true,
                Mensagem = $"Entrada autorizada para {colab.Nome} ({colab.Tipo})."
            };
        }
        else
        {
            var ultimoAcesso = acessosHoje.Count > 0 ? acessosHoje[^1] : null;
            var sessao = ultimoAcesso != null ? (DateTime.Now - ultimoAcesso.DataHora) : TimeSpan.Zero;
            if (sessao < TimeSpan.Zero) sessao = TimeSpan.Zero;

            var novoAcesso = AcessoColaborador.Criar(0, colaboradorId, DateTime.Now);
            await _acessoColaboradorRepo.Adicionar(novoAcesso.Value!, cancellationToken);

            var totalTrabalhado = await _acessoColaboradorRepo.ObterHorasTrabalhadasNoDia(colaboradorId, hoje, cancellationToken);
            string tempoSessaoStr = sessao.ToString(@"hh\:mm\:ss");
            string tempoTotalStr = totalTrabalhado.ToString(@"hh\:mm\:ss");

            return new AcessoResultadoDto
            {
                Sucesso = true,
                EstaDentro = false,
                TempoPermanencia = sessao,
                TempoTotalDia = totalTrabalhado,
                Mensagem = $"Saída registrada. Tempo da sessão: {tempoSessaoStr} | Total hoje: {tempoTotalStr}"
            };
        }
    }

    public async Task<IEnumerable<AcessoHistoricoDto>> ObterAcessosHojeAsync(CancellationToken cancellationToken = default)
    {
        var hoje = DateOnly.FromDateTime(DateTime.Today);
        var acessosAlunos = (await _acessoAlunoRepo.ObterAcessosPorAlunoPeriodo(null, hoje, hoje, cancellationToken)).ToList();
        var acessosColabs = (await _acessoColaboradorRepo.ObterAcessosPorColaboradorPeriodo(null, hoje, hoje, cancellationToken)).ToList();

        var todosAlunos = (await _alunoService.ObterTodosAsync(cancellationToken)).ToDictionary(a => a.Id);
        var todosColabs = (await _colaboradorService.ObterTodosAsync(cancellationToken)).ToDictionary(c => c.Id);

        var lista = new List<AcessoHistoricoDto>();

        var gruposAluno = acessosAlunos.GroupBy(a => a.AlunoId);
        foreach (var grupo in gruposAluno)
        {
            var ordenados = grupo.OrderBy(a => a.DataHora).ToList();
            todosAlunos.TryGetValue(grupo.Key, out var aluno);

            for (int i = 0; i < ordenados.Count; i++)
            {
                var item = ordenados[i];
                bool isEntrada = (i % 2 == 0);
                string tipoAcesso = isEntrada ? "Entrada" : "Saída";
                string detalhe = isEntrada ? "Acesso Aluno" : "Saída Aluno";

                lista.Add(new AcessoHistoricoDto
                {
                    Id = item.Id,
                    PessoaId = item.AlunoId,
                    PessoaTipo = 0,
                    Nome = aluno?.Nome ?? $"Aluno #{item.AlunoId}",
                    Cpf = aluno?.Cpf ?? string.Empty,
                    DataHora = item.DataHora,
                    TipoAcesso = tipoAcesso,
                    Detalhes = detalhe
                });
            }
        }

        var gruposColab = acessosColabs.GroupBy(c => c.ColaboradorId);
        foreach (var grupo in gruposColab)
        {
            var ordenados = grupo.OrderBy(c => c.DataHora).ToList();
            todosColabs.TryGetValue(grupo.Key, out var colab);

            for (int i = 0; i < ordenados.Count; i++)
            {
                var item = ordenados[i];
                bool isEntrada = (i % 2 == 0);
                string tipoAcesso = isEntrada ? "Entrada" : "Saída";
                string detalhe = isEntrada ? "Acesso Colaborador" : "Saída Colaborador";

                lista.Add(new AcessoHistoricoDto
                {
                    Id = item.Id,
                    PessoaId = item.ColaboradorId,
                    PessoaTipo = 1,
                    Nome = colab?.Nome ?? $"Colaborador #{item.ColaboradorId}",
                    Cpf = colab?.Cpf ?? string.Empty,
                    DataHora = item.DataHora,
                    TipoAcesso = tipoAcesso,
                    Detalhes = detalhe
                });
            }
        }

        return lista.OrderByDescending(l => l.DataHora);
    }
}


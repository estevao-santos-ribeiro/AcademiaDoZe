using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Enums;
using AcademiaDoZe.Application.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace AcademiaDoZe.Presentation.AppMaui.ViewModels;

public class PessoaLookupItem
{
    public int Id { get; set; }
    public int Tipo { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public byte[]? Foto { get; set; }
    public string Detalhe { get; set; } = string.Empty;
    public string DisplayText => $"{Nome} - CPF: {Cpf}";
}

public partial class CatracaViewModel : BaseViewModel
{
    private readonly IAcessoService _acessoService;
    private readonly IAlunoService _alunoService;
    private readonly IColaboradorService _colaboradorService;
    private readonly IMatriculaService _matriculaService;

    private List<PessoaLookupItem> _todasPessoas = [];

    [ObservableProperty]
    private bool _isAlunoMode = true;

    [ObservableProperty]
    private bool _isColaboradorMode = false;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private ObservableCollection<PessoaLookupItem> _pessoasFiltradas = [];

    [ObservableProperty]
    private PessoaLookupItem? _selectedPessoa;

    [ObservableProperty]
    private bool _hasSelectedPessoa;

    [ObservableProperty]
    private string _nomePessoa = string.Empty;

    [ObservableProperty]
    private string _cpfPessoa = string.Empty;

    [ObservableProperty]
    private string _tipoPessoaDescricao = "ALUNO";

    [ObservableProperty]
    private byte[]? _fotoPessoa;

    [ObservableProperty]
    private string _infoComplementar = string.Empty;

    [ObservableProperty]
    private bool _estaDentro;

    [ObservableProperty]
    private string _statusPresencaTexto = "FORA DA ACADEMIA";

    [ObservableProperty]
    private string _statusPresencaCor = "#64748B";

    [ObservableProperty]
    private string _botaoAcaoTexto = "REGISTRAR ENTRADA";

    [ObservableProperty]
    private string _botaoAcaoIcone = "\ue899";

    [ObservableProperty]
    private string _botaoAcaoCor = "#EA580C";

    [ObservableProperty]
    private bool _feedbackVisible;

    [ObservableProperty]
    private bool _feedbackSucesso;

    [ObservableProperty]
    private string _feedbackTitulo = string.Empty;

    [ObservableProperty]
    private string _feedbackMensagem = string.Empty;

    [ObservableProperty]
    private string _feedbackIcone = "\ue86c";

    [ObservableProperty]
    private ObservableCollection<AcessoHistoricoDto> _acessosHoje = [];

    public CatracaViewModel(
        IAcessoService acessoService,
        IAlunoService alunoService,
        IColaboradorService colaboradorService,
        IMatriculaService matriculaService)
    {
        _acessoService = acessoService;
        _alunoService = alunoService;
        _colaboradorService = colaboradorService;
        _matriculaService = matriculaService;
        Title = "Controle de Acesso / Catraca";
    }

    public async Task InicializarAsync()
    {
        await CarregarPessoasAsync();
        await CarregarAcessosHojeAsync();
    }

    [RelayCommand]
    private async Task AlternarParaAlunoAsync()
    {
        if (IsAlunoMode) return;
        IsAlunoMode = true;
        IsColaboradorMode = false;
        LimparSelecao();
        await CarregarPessoasAsync();
    }

    [RelayCommand]
    private async Task AlternarParaColaboradorAsync()
    {
        if (IsColaboradorMode) return;
        IsAlunoMode = false;
        IsColaboradorMode = true;
        LimparSelecao();
        await CarregarPessoasAsync();
    }

    [RelayCommand]
    private async Task CarregarPessoasAsync()
    {
        try
        {
            IsBusy = true;
            _todasPessoas.Clear();

            if (IsAlunoMode)
            {
                var alunos = await _alunoService.ObterTodosAsync();
                foreach (var a in alunos)
                {
                    _todasPessoas.Add(new PessoaLookupItem
                    {
                        Id = a.Id,
                        Tipo = 0,
                        Nome = a.Nome,
                        Cpf = a.Cpf,
                        Foto = a.Foto?.Conteudo,
                        Detalhe = $"Nasc: {a.DataNascimento:dd/MM/yyyy}"
                    });
                }
            }
            else
            {
                var colabs = await _colaboradorService.ObterTodosAsync();
                foreach (var c in colabs)
                {
                    _todasPessoas.Add(new PessoaLookupItem
                    {
                        Id = c.Id,
                        Tipo = 1,
                        Nome = c.Nome,
                        Cpf = c.Cpf,
                        Foto = c.Foto?.Conteudo,
                        Detalhe = $"{c.Tipo} • {c.Vinculo}"
                    });
                }
            }

            FiltrarPessoas();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Erro", $"Falha ao carregar pessoas: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    partial void OnSearchTextChanged(string value)
    {
        FiltrarPessoas();
    }

    private void FiltrarPessoas()
    {
        var termo = SearchText?.Trim().ToLowerInvariant() ?? string.Empty;
        var lista = string.IsNullOrWhiteSpace(termo)
            ? _todasPessoas
            : _todasPessoas.Where(p => p.Nome.ToLowerInvariant().Contains(termo) || p.Cpf.Contains(termo)).ToList();

        PessoasFiltradas = new ObservableCollection<PessoaLookupItem>(lista);
    }

    partial void OnSelectedPessoaChanged(PessoaLookupItem? value)
    {
        if (value == null)
        {
            LimparSelecao();
            return;
        }

        HasSelectedPessoa = true;
        NomePessoa = value.Nome;
        CpfPessoa = value.Cpf;
        TipoPessoaDescricao = value.Tipo == 0 ? "ALUNO" : "COLABORADOR";
        FotoPessoa = value.Foto;
        FeedbackVisible = false;

        _ = AtualizarStatusPessoaSelecionadaAsync(value);
    }

    private async Task AtualizarStatusPessoaSelecionadaAsync(PessoaLookupItem pessoa)
    {
        try
        {
            if (pessoa.Tipo == 0)
            {
                var status = await _acessoService.VerificarStatusAlunoAsync(pessoa.Id);
                EstaDentro = status.EstaDentro;

                var matricula = await _matriculaService.ObterMatriculaAtivaPorAlunoAsync(pessoa.Id);
                if (matricula != null)
                {
                    int dias = Math.Max(0, (matricula.DataFim.ToDateTime(TimeOnly.MinValue) - DateTime.Today).Days);
                    InfoComplementar = $"Matrícula Ativa ({matricula.Plano}) • {dias} dias restantes";
                }
                else
                {
                    InfoComplementar = "Sem matrícula ativa no momento";
                }
            }
            else
            {
                var status = await _acessoService.VerificarStatusColaboradorAsync(pessoa.Id);
                EstaDentro = status.EstaDentro;

                var colab = await _colaboradorService.ObterPorIdAsync(pessoa.Id);
                string tempoTotal = status.TempoTotalDia?.ToString(@"hh\:mm\:ss") ?? "00:00:00";
                InfoComplementar = $"Função: {colab?.Tipo} • Vínculo: {colab?.Vinculo} • Hoje: {tempoTotal}";
            }

            AtualizarTextosEstado();
        }
        catch (Exception ex)
        {
            InfoComplementar = $"Erro ao verificar status: {ex.Message}";
        }
    }

    private void AtualizarTextosEstado()
    {
        if (EstaDentro)
        {
            StatusPresencaTexto = "DENTRO DA ACADEMIA";
            StatusPresencaCor = "#10B981";
            BotaoAcaoTexto = "REGISTRAR SAÍDA";
            BotaoAcaoIcone = "\ue879";
            BotaoAcaoCor = "#EF4444";
        }
        else
        {
            StatusPresencaTexto = "FORA DA ACADEMIA";
            StatusPresencaCor = "#64748B";
            BotaoAcaoTexto = "REGISTRAR ENTRADA";
            BotaoAcaoIcone = "\ue899";
            BotaoAcaoCor = "#EA580C";
        }
    }

    private void LimparSelecao()
    {
        SelectedPessoa = null;
        HasSelectedPessoa = false;
        NomePessoa = string.Empty;
        CpfPessoa = string.Empty;
        TipoPessoaDescricao = IsAlunoMode ? "ALUNO" : "COLABORADOR";
        FotoPessoa = null;
        InfoComplementar = string.Empty;
        EstaDentro = false;
        FeedbackVisible = false;
        AtualizarTextosEstado();
    }

    [RelayCommand]
    private async Task RegistrarAcessoAsync()
    {
        if (SelectedPessoa == null)
        {
            await Shell.Current.DisplayAlert("Atenção", "Selecione uma pessoa para registrar o acesso.", "OK");
            return;
        }

        try
        {
            IsBusy = true;
            AcessoResultadoDto resultado;

            if (SelectedPessoa.Tipo == 0)
            {
                resultado = await _acessoService.RegistrarAcessoAlunoAsync(SelectedPessoa.Id);
            }
            else
            {
                resultado = await _acessoService.RegistrarAcessoColaboradorAsync(SelectedPessoa.Id);
            }

            FeedbackVisible = true;
            FeedbackSucesso = resultado.Sucesso;
            FeedbackMensagem = resultado.Mensagem;

            if (resultado.Sucesso)
            {
                EstaDentro = resultado.EstaDentro;
                FeedbackTitulo = resultado.EstaDentro ? "ENTRADA AUTORIZADA" : "SAÍDA REGISTRADA";
                FeedbackIcone = "\ue86c";
                AtualizarTextosEstado();
                await AtualizarStatusPessoaSelecionadaAsync(SelectedPessoa);
                await CarregarAcessosHojeAsync();
            }
            else
            {
                FeedbackTitulo = "ACESSO NEGADO";
                FeedbackIcone = "\ue000";
            }
        }
        catch (Exception ex)
        {
            FeedbackVisible = true;
            FeedbackSucesso = false;
            FeedbackTitulo = "ERRO NO PROCESSAMENTO";
            FeedbackMensagem = ex.Message;
            FeedbackIcone = "\ue000";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task CarregarAcessosHojeAsync()
    {
        try
        {
            IsRefreshing = true;
            var lista = await _acessoService.ObterAcessosHojeAsync();
            AcessosHoje = new ObservableCollection<AcessoHistoricoDto>(lista);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Erro", $"Falha ao carregar acessos de hoje: {ex.Message}", "OK");
        }
        finally
        {
            IsRefreshing = false;
        }
    }
}


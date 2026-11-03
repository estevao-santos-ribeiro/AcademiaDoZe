using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Enums;
using AcademiaDoZe.Application.Interfaces;
using CommunityToolkit.Mvvm.Input;

namespace AcademiaDoZe.Presentation.AppMaui.ViewModels;

[QueryProperty(nameof(ColaboradorId), "Id")]
public partial class ColaboradorViewModel : BaseViewModel
{
    private readonly IColaboradorService _colaboradorService;
    private readonly ILogradouroService _logradouroService;

    public IEnumerable<AppColaboradorTipo> Tipos { get; } = Enum.GetValues<AppColaboradorTipo>();
    public IEnumerable<AppColaboradorVinculo> Vinculos { get; } = Enum.GetValues<AppColaboradorVinculo>();

    private ColaboradorDto _colaborador = new()
    {
        Nome = string.Empty,
        Cpf = string.Empty,
        DataNascimento = DateOnly.FromDateTime(DateTime.Today.AddYears(-20)),
        Telefone = string.Empty,
        Email = string.Empty,
        Endereco = new LogradouroDto
        {
            Cep = string.Empty,
            Nome = string.Empty,
            Bairro = string.Empty,
            Cidade = string.Empty,
            Estado = string.Empty,
            Pais = string.Empty
        },
        Numero = string.Empty,
        Complemento = string.Empty,
        Senha = string.Empty,
        DataAdmissao = DateOnly.FromDateTime(DateTime.Today),
        Tipo = AppColaboradorTipo.Instrutor,
        Vinculo = AppColaboradorVinculo.CLT
    };

    public ColaboradorDto Colaborador
    {
        get => _colaborador;
        set => SetProperty(ref _colaborador, value);
    }

    private int _colaboradorId;
    public int ColaboradorId
    {
        get => _colaboradorId;
        set => SetProperty(ref _colaboradorId, value);
    }

    private bool _isEditMode;
    public bool IsEditMode
    {
        get => _isEditMode;
        set => SetProperty(ref _isEditMode, value);
    }

    public ColaboradorViewModel(IColaboradorService colaboradorService, ILogradouroService logradouroService)
    {
        _colaboradorService = colaboradorService;
        _logradouroService = logradouroService;
        Title = "Colaborador";
    }

    public async Task InitializeAsync()
    {
        if (ColaboradorId > 0)
        {
            IsEditMode = true;
            Title = "Editar Colaborador";
            await LoadColaboradorAsync();
        }
        else
        {
            IsEditMode = false;
            Title = "Novo Colaborador";
        }
    }

    [RelayCommand]
    public async Task LoadColaboradorAsync()
    {
        if (ColaboradorId <= 0)
            return;
        try
        {
            IsBusy = true;
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var data = await _colaboradorService.ObterPorIdAsync(ColaboradorId, cts.Token);
            if (data != null)
            {
                Colaborador = data;
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync("Erro", $"Erro ao carregar colaborador: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task SearchByCpfAsync()
    {
        if (string.IsNullOrWhiteSpace(Colaborador.Cpf))
            return;
        try
        {
            IsBusy = true;
            // normaliza para apenas dígitos (o repositório espera dígitos)
            var cpfNormalized = new string(Colaborador.Cpf.Where(char.IsDigit).ToArray());
            var resultados = (await _colaboradorService.ObterPorCpfAsync(cpfNormalized))?.ToList() ?? new List<ColaboradorDto>();
            if (!resultados.Any())
            {
                await Shell.Current.DisplayAlertAsync("Aviso", "CPF não encontrado.", "OK");
                return;
            }
            if (resultados.Count == 1)
            {
                Colaborador = resultados.First();
                IsEditMode = true;
                await Shell.Current.DisplayAlertAsync("Aviso", "Colaborador já cadastrado! Dados carregados para edição.", "OK");
                return;
            }
            // múltiplos resultados -> perguntar ao usuário qual selecionar
            var options = resultados.Select(c => $"{c.Id} - {c.Nome} ({c.Cpf})").ToArray();
            var escolha = await Shell.Current.DisplayActionSheetAsync("Vários colaboradores encontrados", "Cancelar", null, options);
            if (string.IsNullOrWhiteSpace(escolha) || escolha == "Cancelar")
                return;
            // extrai ID a partir da string selecionada ("{Id} - ...")
            var idStr = escolha.Split('-', 2).FirstOrDefault()?.Trim();
            if (int.TryParse(idStr, out var selId))
            {
                var selecionado = resultados.FirstOrDefault(c => c.Id == selId);
                if (selecionado != null)
                {
                    Colaborador = selecionado;
                    IsEditMode = true;
                    await Shell.Current.DisplayAlertAsync("Aviso", "Colaborador selecionado: dados carregados para edição.", "OK");
                }
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync("Erro", $"Erro ao buscar CPF: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task SearchByCepAsync()
    {
        if (string.IsNullOrWhiteSpace(Colaborador.Endereco?.Cep))
            return;
        try
        {
            IsBusy = true;
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var cepLimpo = new string(Colaborador.Endereco.Cep.Where(char.IsDigit).ToArray());
            var logradouro = await _logradouroService.ObterPorCepAsync(cepLimpo, cts.Token);
            if (logradouro != null)
            {
                Colaborador.Endereco = logradouro;
                OnPropertyChanged(nameof(Colaborador));
                await Shell.Current.DisplayAlertAsync("Aviso", "Endereço localizado e preenchido!", "OK");
            }
            else
            {
                await Shell.Current.DisplayAlertAsync("Aviso", "CEP não localizado no cadastro.", "OK");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync("Erro", $"Erro ao buscar CEP: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task SelecionarFotoAsync()
    {
        try
        {
            string escolha = await Shell.Current.DisplayActionSheetAsync("Origem da Foto", "Cancelar", null, "Galeria", "Câmera", "Remover Foto");
            if (escolha == "Remover Foto")
            {
                Colaborador.Foto = null;
                OnPropertyChanged(nameof(Colaborador));
                return;
            }

            FileResult? result = null;
            if (escolha == "Galeria")
            {
                result = await FilePicker.Default.PickAsync(new PickOptions
                {
                    PickerTitle = "Selecione a Foto",
                    FileTypes = FilePickerFileType.Images
                });
            }
            else if (escolha == "Câmera")
            {
                if (MediaPicker.Default.IsCaptureSupported)
                {
                    result = await MediaPicker.Default.CapturePhotoAsync();
                }
                else
                {
                    await Shell.Current.DisplayAlertAsync("Erro", "Câmera não suportada neste dispositivo.", "OK");
                    return;
                }
            }

            if (result != null)
            {
                using var stream = await result.OpenReadAsync();
                using var ms = new MemoryStream();
                await stream.CopyToAsync(ms);
                Colaborador.Foto = new ArquivoDto { Conteudo = ms.ToArray() };
                OnPropertyChanged(nameof(Colaborador));
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync("Erro", $"Erro ao selecionar foto: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    public async Task SaveColaboradorAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

            if (IsEditMode)
            {
                await _colaboradorService.AtualizarAsync(Colaborador, cts.Token);
                await Shell.Current.DisplayAlertAsync("Sucesso", "Colaborador atualizado com sucesso!", "OK");
            }
            else
            {
                await _colaboradorService.AdicionarAsync(Colaborador, cts.Token);
                await Shell.Current.DisplayAlertAsync("Sucesso", "Colaborador cadastrado com sucesso!", "OK");
            }

            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync("Erro", $"Erro ao salvar colaborador: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task CancelAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}


using AcademiaDoZe.Application.DTOs;

namespace AcademiaDoZe.Application.Interfaces;

public interface IAcessoService
{
    Task<AcessoResultadoDto> VerificarStatusAlunoAsync(int alunoId, CancellationToken cancellationToken = default);
    Task<AcessoResultadoDto> RegistrarAcessoAlunoAsync(int alunoId, CancellationToken cancellationToken = default);
    Task<AcessoResultadoDto> VerificarStatusColaboradorAsync(int colaboradorId, CancellationToken cancellationToken = default);
    Task<AcessoResultadoDto> RegistrarAcessoColaboradorAsync(int colaboradorId, CancellationToken cancellationToken = default);
    Task<IEnumerable<AcessoHistoricoDto>> ObterAcessosHojeAsync(CancellationToken cancellationToken = default);
}


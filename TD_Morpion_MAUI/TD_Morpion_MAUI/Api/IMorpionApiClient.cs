using TD_Morpion_MAUI.Api.Models;

namespace TD_Morpion_MAUI.Api;

public interface IMorpionApiClient
{
	Task<IReadOnlyCollection<GameResponseDto>> GetGamesAsync(CancellationToken cancellationToken = default);

	Task<GameResponseDto> CreateGameAsync(CancellationToken cancellationToken = default);

	Task<GameResponseDto> PlayMoveAsync(Guid gameId, int position, CancellationToken cancellationToken = default);
}

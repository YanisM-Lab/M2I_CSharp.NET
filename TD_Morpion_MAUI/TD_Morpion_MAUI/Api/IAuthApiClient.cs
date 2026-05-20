using TD_Morpion_MAUI.Api.Models;

namespace TD_Morpion_MAUI.Api;

public interface IAuthApiClient
{
	Task<AuthResponseDto> LoginAsync(string email, string password, CancellationToken cancellationToken = default);

	Task<AuthResponseDto> RegisterAsync(string userName, string email, string password, CancellationToken cancellationToken = default);
}

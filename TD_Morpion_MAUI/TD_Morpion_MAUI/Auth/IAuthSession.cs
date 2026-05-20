using TD_Morpion_MAUI.Api.Models;

namespace TD_Morpion_MAUI.Auth;

public interface IAuthSession
{
	AuthResponseDto? CurrentUser { get; }

	bool IsAuthenticated { get; }

	Task<bool> LoadAsync();

	Task SaveAsync(AuthResponseDto authResponse);

	void Clear();
}

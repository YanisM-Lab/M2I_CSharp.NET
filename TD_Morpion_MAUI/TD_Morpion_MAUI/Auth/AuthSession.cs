using System.Globalization;
using TD_Morpion_MAUI.Api.Models;

namespace TD_Morpion_MAUI.Auth;

public sealed class AuthSession : IAuthSession
{
	private const string TokenKey = "auth_token";
	private const string ExpiresAtKey = "auth_expires_at";
	private const string UserIdKey = "auth_user_id";
	private const string UserNameKey = "auth_user_name";
	private const string EmailKey = "auth_email";

	public AuthResponseDto? CurrentUser { get; private set; }

	public bool IsAuthenticated =>
		CurrentUser is not null && CurrentUser.ExpiresAtUtc > DateTime.UtcNow;

	public async Task<bool> LoadAsync()
	{
		var token = await SecureStorage.Default.GetAsync(TokenKey);
		var expiresAtValue = await SecureStorage.Default.GetAsync(ExpiresAtKey);
		var userId = await SecureStorage.Default.GetAsync(UserIdKey);
		var userName = await SecureStorage.Default.GetAsync(UserNameKey);
		var email = await SecureStorage.Default.GetAsync(EmailKey);

		if (string.IsNullOrWhiteSpace(token)
			|| string.IsNullOrWhiteSpace(expiresAtValue)
			|| string.IsNullOrWhiteSpace(userId)
			|| string.IsNullOrWhiteSpace(userName)
			|| string.IsNullOrWhiteSpace(email)
			|| !DateTime.TryParse(expiresAtValue, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var expiresAtUtc))
		{
			Clear();
			return false;
		}

		CurrentUser = new AuthResponseDto(token, expiresAtUtc, userId, userName, email);

		if (IsAuthenticated)
		{
			return true;
		}

		Clear();
		return false;
	}

	public async Task SaveAsync(AuthResponseDto authResponse)
	{
		CurrentUser = authResponse;

		await SecureStorage.Default.SetAsync(TokenKey, authResponse.Token);
		await SecureStorage.Default.SetAsync(ExpiresAtKey, authResponse.ExpiresAtUtc.ToString("O", CultureInfo.InvariantCulture));
		await SecureStorage.Default.SetAsync(UserIdKey, authResponse.UserId);
		await SecureStorage.Default.SetAsync(UserNameKey, authResponse.UserName);
		await SecureStorage.Default.SetAsync(EmailKey, authResponse.Email);
	}

	public void Clear()
	{
		CurrentUser = null;
		SecureStorage.Default.Remove(TokenKey);
		SecureStorage.Default.Remove(ExpiresAtKey);
		SecureStorage.Default.Remove(UserIdKey);
		SecureStorage.Default.Remove(UserNameKey);
		SecureStorage.Default.Remove(EmailKey);
	}
}

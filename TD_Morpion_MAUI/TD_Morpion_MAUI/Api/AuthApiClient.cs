using System.Net.Http.Json;
using System.Text.Json;
using TD_Morpion_MAUI.Api.Models;

namespace TD_Morpion_MAUI.Api;

public sealed class AuthApiClient(HttpClient httpClient) : IAuthApiClient
{
	private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

	public async Task<AuthResponseDto> LoginAsync(
		string email,
		string password,
		CancellationToken cancellationToken = default)
	{
		using var response = await httpClient.PostAsJsonAsync(
			"api/auth/login",
			new LoginRequestDto(email, password),
			JsonOptions,
			cancellationToken);

		return await ReadAuthResponseAsync(response, cancellationToken);
	}

	public async Task<AuthResponseDto> RegisterAsync(
		string userName,
		string email,
		string password,
		CancellationToken cancellationToken = default)
	{
		using var response = await httpClient.PostAsJsonAsync(
			"api/auth/register",
			new RegisterRequestDto(userName, email, password),
			JsonOptions,
			cancellationToken);

		return await ReadAuthResponseAsync(response, cancellationToken);
	}

	private static async Task<AuthResponseDto> ReadAuthResponseAsync(
		HttpResponseMessage response,
		CancellationToken cancellationToken)
	{
		if (response.IsSuccessStatusCode)
		{
			var auth = await response.Content.ReadFromJsonAsync<AuthResponseDto>(JsonOptions, cancellationToken);
			return auth ?? throw new InvalidOperationException("La reponse de l'API est vide.");
		}

		var apiError = await response.Content.ReadFromJsonAsync<ApiErrorResponse>(JsonOptions, cancellationToken);
		var message = apiError?.Message ?? $"Erreur API {(int)response.StatusCode}.";
		throw new InvalidOperationException(message);
	}
}

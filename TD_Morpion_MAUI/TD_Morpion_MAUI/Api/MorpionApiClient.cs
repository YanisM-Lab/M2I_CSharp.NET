using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using TD_Morpion_MAUI.Api.Models;
using TD_Morpion_MAUI.Auth;

namespace TD_Morpion_MAUI.Api;

public sealed class MorpionApiClient(HttpClient httpClient, IAuthSession authSession) : IMorpionApiClient
{
	private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

	public async Task<IReadOnlyCollection<GameResponseDto>> GetGamesAsync(CancellationToken cancellationToken = default)
	{
		using var request = await CreateAuthenticatedRequestAsync(HttpMethod.Get, "api/games");
		using var response = await httpClient.SendAsync(request, cancellationToken);

		if (response.IsSuccessStatusCode)
		{
			var games = await response.Content.ReadFromJsonAsync<IReadOnlyCollection<GameResponseDto>>(
				JsonOptions,
				cancellationToken);

			return games ?? [];
		}

		await ThrowApiExceptionAsync(response, cancellationToken);
		return [];
	}

	public async Task<GameResponseDto> CreateGameAsync(CancellationToken cancellationToken = default)
	{
		using var request = await CreateAuthenticatedRequestAsync(HttpMethod.Post, "api/games");
		request.Content = JsonContent.Create(new { }, options: JsonOptions);
		using var response = await httpClient.SendAsync(request, cancellationToken);

		return await ReadGameResponseAsync(response, cancellationToken);
	}

	public async Task<GameResponseDto> PlayMoveAsync(Guid gameId, int position, CancellationToken cancellationToken = default)
	{
		using var request = await CreateAuthenticatedRequestAsync(HttpMethod.Post, $"api/games/{gameId}/moves");
		request.Content = JsonContent.Create(new PlayMoveRequestDto(position), options: JsonOptions);
		using var response = await httpClient.SendAsync(request, cancellationToken);

		return await ReadGameResponseAsync(response, cancellationToken);
	}

	private async Task<HttpRequestMessage> CreateAuthenticatedRequestAsync(HttpMethod method, string uri)
	{
		if (!authSession.IsAuthenticated)
		{
			await authSession.LoadAsync();
		}

		var token = authSession.CurrentUser?.Token;
		if (string.IsNullOrWhiteSpace(token))
		{
			throw new UnauthorizedAccessException("Vous devez vous connecter.");
		}

		var request = new HttpRequestMessage(method, uri);
		request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
		return request;
	}

	private static async Task<GameResponseDto> ReadGameResponseAsync(
		HttpResponseMessage response,
		CancellationToken cancellationToken)
	{
		if (response.IsSuccessStatusCode)
		{
			var game = await response.Content.ReadFromJsonAsync<GameResponseDto>(JsonOptions, cancellationToken);
			return game ?? throw new InvalidOperationException("La reponse de l'API est vide.");
		}

		await ThrowApiExceptionAsync(response, cancellationToken);
		throw new InvalidOperationException("Erreur API.");
	}

	private static async Task ThrowApiExceptionAsync(
		HttpResponseMessage response,
		CancellationToken cancellationToken)
	{
		var apiError = await response.Content.ReadFromJsonAsync<ApiErrorResponse>(JsonOptions, cancellationToken);
		var message = apiError?.Message ?? $"Erreur API {(int)response.StatusCode}.";

		if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
		{
			throw new UnauthorizedAccessException(message);
		}

		throw new InvalidOperationException(message);
	}
}

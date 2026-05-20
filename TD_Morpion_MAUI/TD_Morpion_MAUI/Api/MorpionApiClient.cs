using System.Net.Http.Json;
using System.Text.Json;
using TD_Morpion_MAUI.Api.Models;

namespace TD_Morpion_MAUI.Api;

public sealed class MorpionApiClient(HttpClient httpClient) : IMorpionApiClient
{
	private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

	public async Task<GameResponseDto> CreateGameAsync(CancellationToken cancellationToken = default)
	{
		using var response = await httpClient.PostAsJsonAsync("api/games", new { }, JsonOptions, cancellationToken);
		return await ReadGameResponseAsync(response, cancellationToken);
	}

	public async Task<GameResponseDto> PlayMoveAsync(Guid gameId, int position, CancellationToken cancellationToken = default)
	{
		using var response = await httpClient.PostAsJsonAsync(
			$"api/games/{gameId}/moves",
			new PlayMoveRequestDto(position),
			JsonOptions,
			cancellationToken);

		return await ReadGameResponseAsync(response, cancellationToken);
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

		var apiError = await response.Content.ReadFromJsonAsync<ApiErrorResponse>(JsonOptions, cancellationToken);
		var message = apiError?.Message ?? $"Erreur API {(int)response.StatusCode}.";
		throw new InvalidOperationException(message);
	}
}

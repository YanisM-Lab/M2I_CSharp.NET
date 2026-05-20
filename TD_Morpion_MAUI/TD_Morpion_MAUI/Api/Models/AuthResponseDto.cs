namespace TD_Morpion_MAUI.Api.Models;

public sealed record AuthResponseDto(
	string Token,
	DateTime ExpiresAtUtc,
	string UserId,
	string UserName,
	string Email);

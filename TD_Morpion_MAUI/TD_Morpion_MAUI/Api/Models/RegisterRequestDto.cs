namespace TD_Morpion_MAUI.Api.Models;

public sealed record RegisterRequestDto(
	string UserName,
	string Email,
	string Password);

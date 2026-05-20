namespace TD_Morpion_MAUI.Api.Models;

public sealed record MoveResponseDto(
	Guid Id,
	string PlayerSymbol,
	int Position,
	DateTime PlayedAtUtc);

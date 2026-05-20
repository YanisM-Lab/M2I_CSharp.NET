namespace TD_Morpion_MAUI.Api.Models;

public sealed record GameResponseDto(
	Guid Id,
	string Board,
	string HumanSymbol,
	string BotSymbol,
	string NextPlayer,
	string Status,
	string? Winner,
	DateTime CreatedAtUtc,
	DateTime UpdatedAtUtc,
	MoveResponseDto? LastHumanMove,
	MoveResponseDto? LastBotMove,
	IReadOnlyCollection<MoveResponseDto> Moves);

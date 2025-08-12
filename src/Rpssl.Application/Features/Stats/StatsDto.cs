namespace Rpssl.Application.Features.Stats;

public sealed record StatsDto(
    int TotalGames,
    int PlayerWins,
    int ComputerWins,
    int Draws
);

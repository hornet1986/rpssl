namespace Rpssl.Application.Stats;

public sealed record StatsDto(
    int TotalGames,
    int PlayerWins,
    int ComputerWins,
    int Draws
);

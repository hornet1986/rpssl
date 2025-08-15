namespace Rpssl.Application.GameResults;

public sealed record GameResultsDto(
    int TotalGames,
    int PlayerWins,
    int ComputerWins,
    int Draws
);

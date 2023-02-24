using CodeBreaker.Data.Models;
using CodeBreaker.Shared.Api;

namespace CodeBreaker.APIs.Extensions;

public static class DtoExtensions
{
    public static GameDto ToDto(this Game game) =>
        new(
            game.GameId,
            game.Code,
            game.Holes,
            game.Colors,
            game.MaxMoves,
            game.Username,
            game.Start,
            game.End,
            game.Moves.Select(m => m.ToDto())
        );

    public static MoveDto ToDto(this Move move) =>
        new(
            move.MoveNumber,
            move.GuessPegs,
            move.KeyPegs?.ToDto()
        );

    public static KeyPegsDto ToDto(this KeyPegs keyPegs) =>
        new(keyPegs.Black, keyPegs.White);

    public static KeyPegs ToModel(this KeyPegsDto dto) =>
        new(dto.Black, dto.White);

    public static Game ToModel(this GameDto dto) =>
        new()
        {
            GameId = dto.GameId,
            Username = dto.Username,
            Code = dto.Code,
            Start = dto.Start,
            End = dto.End,
            Moves = dto.Moves.Select(m => m.ToModel()).ToList(),
            Colors = dto.Colors,
            MaxMoves = dto.MaxMoves,
        };

    public static Move ToModel(this MoveDto dto) =>
        new()
        {
            MoveNumber = dto.MoveNumber,
            GuessPegs = dto.GuessPegs,
            KeyPegs = dto.KeyPegs?.ToModel(),
        };
}

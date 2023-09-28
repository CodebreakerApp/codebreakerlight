using CodeBreaker.Shared.Models.Api;
using CodeBreaker.Shared.Models.Data;

namespace CodeBreaker.Shared.Models.Extensions;

public static class DtoExtensions
{
    public static GameDto ToDto(this Game game) =>
        new()
        {
            GameId = game.GameId,
            Type = game.Type,
            Code = game.Code,
            Username = game.Username,
            Start = game.Start,
            End = game.End,
            Moves = game.Moves.Select(m => m.ToDto())
        };

    public static MoveDto<TField> ToDto<TField>(this Move<TField> move) =>
        new()
        {
            MoveNumber = move.MoveNumber,
            GuessPegs = move.GuessPegs,
            KeyPegs = move.KeyPegs?.ToDto()
        };

    public static KeyPegsDto ToDto(this KeyPegs keyPegs) =>
        new()
        {
            Black = keyPegs.Black,
            White = keyPegs.White
        };

    public static KeyPegs ToModel(this KeyPegsDto dto) =>
        new(dto.Black, dto.White);

    public static Game ToModel(this GameDto dto) =>
        new(
            dto.GameId,
            dto.Type,
            dto.Username,
            dto.Code,
            dto.Start,
            dto.End,
            dto.Moves.Select(m => m.ToModel()).ToList()
        );

    public static Move ToModel(this MoveDto<string> dto) =>
        new(
            dto.MoveNumber,
            dto.GuessPegs,
            dto.KeyPegs?.ToModel()
        );

    public static GameTypeDto ToDto<TField>(this GameType<TField> gameType) =>
        new()
        {
            Name = gameType.Name
        };
}

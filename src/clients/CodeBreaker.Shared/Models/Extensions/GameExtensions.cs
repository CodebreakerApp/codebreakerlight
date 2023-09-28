using CodeBreaker.Shared.Models.Data;

namespace CodeBreaker.Shared.Models.Extensions
{
    public static class GameExtensions
    {
        public static Move? GetLastMoveOrDefault(this Game game) =>
            game.Moves.OrderByDescending(x => x.MoveNumber).FirstOrDefault();

        public static KeyPegs? GetLastKeyPegsOrDefault(this Game game) =>
            game.GetLastMoveOrDefault()?.KeyPegs;
    }
}

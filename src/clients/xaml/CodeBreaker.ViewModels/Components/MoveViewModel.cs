using CodeBreaker.Shared.Api;

namespace CodeBreaker.ViewModels.Components;

public class MoveViewModel
{
    private readonly MoveDto _move;

    public MoveViewModel(MoveDto move) =>
        _move = move;

    public int MoveNumber => _move.MoveNumber;

    public IReadOnlyList<string> GuessPegs => _move.GuessPegs;

    public KeyPegsDto? KeyPegs => _move.KeyPegs;
}

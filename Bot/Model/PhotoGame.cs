using System.Collections.Generic;

class PhotoGame : UCB1Tree<PhotoGame, PhotoMove, PhotoPlayer>.IGame
{
    private readonly PhotoPlayer _redPlayer;
    private readonly PhotoPlayer _bluePlayer;

    public PhotoGame(PhotoPlayer redPlayer, PhotoPlayer bluePlayer)
    {
        _redPlayer = redPlayer;
        _bluePlayer = bluePlayer;
    }
    public long Hash => GetHashCode();

    public PhotoPlayer CurrentPlayer => _redPlayer;

    public PhotoGame DeepCopy()
    {
        return new PhotoGame(_redPlayer, _bluePlayer);
    }

    public List<UCB1Tree<PhotoGame, PhotoMove, PhotoPlayer>.Transition> GetLegalTransitions()
    {
        throw new System.NotImplementedException();
    }

    public bool IsGameOver()
    {
        throw new System.NotImplementedException();
    }

    public bool IsWinner(PhotoPlayer player)
    {
        throw new System.NotImplementedException();
    }

    public void Rollout()
    {
        throw new System.NotImplementedException();
    }

    public void Transition(UCB1Tree<PhotoGame, PhotoMove, PhotoPlayer>.Transition t)
    {
        throw new System.NotImplementedException();
    }
}

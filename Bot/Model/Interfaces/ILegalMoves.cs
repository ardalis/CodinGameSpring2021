using System.Collections.Generic;

namespace GameAI.GameInterfaces
{
    /// <summary>
    /// Calculate legal moves here:
    /// https://github.com/CodinGame/SpringChallenge2021/blob/main/src/main/java/com/codingame/game/Game.java
    /// </summary>
    /// <typeparam name="TMove"></typeparam>
    public interface ILegalMoves<TMove>
    {
        /// <summary>
        /// Return a list of legal moves for the current gamestate.
        /// </summary>
        List<TMove> GetLegalMoves();
    }
}

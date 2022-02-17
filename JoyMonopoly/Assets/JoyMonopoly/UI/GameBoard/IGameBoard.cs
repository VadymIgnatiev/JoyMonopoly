using System.Threading.Tasks;
using JoyMonopoly.GameLogic;

namespace JoyMonopoly.UI.GameBoard
{
    public interface IGameBoard
    {
        void SetInitialState();

        Task SetPlayerToCell(PlayersIDs playerID, int targetCell);

        void SetAsPlayerCell(PlayersIDs playersID, int cellNumber);
    }
}
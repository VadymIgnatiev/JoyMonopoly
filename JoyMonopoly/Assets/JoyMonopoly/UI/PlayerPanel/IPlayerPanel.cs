using System.Threading.Tasks;
using JoyMonopoly.GameLogic;

namespace JoyMonopoly.UI.PlayerPanel
{
    public interface IPlayerPanel
    {
        Task SetPlayerMoney(PlayersIDs playersID, int value);

        void SetPlayerAsActive(PlayersIDs playersID, bool isActive);
    }
}
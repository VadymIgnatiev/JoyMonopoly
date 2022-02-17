using System.Threading.Tasks;
using JoyMonopoly.GameLogic;
using TMPro;
using UnityEngine;

namespace JoyMonopoly.UI.PlayerPanel
{
    public class PlayersPanelPresenter : MonoBehaviour, IPlayerPanel
    {
        [SerializeField] private PlayerPanelPresenter _playerOnePanelPresenter;
        [SerializeField] private PlayerPanelPresenter _playerTwoPanelPresenter;

        public async Task SetPlayerMoney(PlayersIDs playersID, int value)
        {
            if (playersID == PlayersIDs.PlayerOne)
                _playerOnePanelPresenter.SetPlayerMoney(value);
            
            if (playersID == PlayersIDs.PlayerTwo)
                _playerTwoPanelPresenter.SetPlayerMoney(value);
        }

        public void SetPlayerAsActive(PlayersIDs playersID, bool isActive)
        {
            if (playersID == PlayersIDs.PlayerOne)
                _playerOnePanelPresenter.SetPlayerAsActive(isActive);
            
            if (playersID == PlayersIDs.PlayerTwo)
                _playerTwoPanelPresenter.SetPlayerAsActive(isActive);
        }
    }
}
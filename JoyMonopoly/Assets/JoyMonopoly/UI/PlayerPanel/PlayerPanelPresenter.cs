using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace JoyMonopoly.UI.PlayerPanel
{
    public class PlayerPanelPresenter : MonoBehaviour
    {
        [SerializeField] private TMP_Text _playerMoneyLabel;
        [SerializeField] private GameObject _activePlayerPanel;

        public async Task SetPlayerMoney(int value)
        {
            _playerMoneyLabel.SetText(value.ToString() + "$");
        }

        public void SetPlayerAsActive(bool isActive)
        {
            _activePlayerPanel.SetActive(isActive);
        }
    }
}
using System;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JoyMonopoly.UI.InformationPopup
{
    public class InformationPopup : MonoBehaviour, IInformationPanel
    {
        [SerializeField] private TMP_Text _informationLabelMessage;
        [SerializeField] private TMP_Text _attantionMessage;
        [SerializeField] private Button _okButton;

        private CancellationTokenSource _source = new CancellationTokenSource();
        private CancellationToken _cancellationToken = new CancellationToken(true);

        private void Awake()
        {
            _okButton.onClick.AddListener(CancelToken);
        }

        public async Task ShowMessages(string informationMessage, string attentionMessage)
        {
            if (!_cancellationToken.IsCancellationRequested) return;

            _source = new CancellationTokenSource();
            _cancellationToken = _source.Token;
            
            _informationLabelMessage.text = informationMessage;
            _attantionMessage.text = attentionMessage;
            
            gameObject.SetActive(true);

            while (!_cancellationToken.IsCancellationRequested)
            {
                await Task.Yield();
            }

            await Hide();
        }

        public async Task Hide()
        {
            gameObject.SetActive(false);
        }

        private void CancelToken()
        {
            _source.Cancel();
        }

        private void OnEnable()
        {
            _okButton.onClick.AddListener(CancelToken);
        }
        
        private void OnDestroy()
        {
            _okButton.onClick.RemoveListener(CancelToken);
        }
        
        private void OnDisable()
        {
            _okButton.onClick.RemoveListener(CancelToken);
        }
    }
}
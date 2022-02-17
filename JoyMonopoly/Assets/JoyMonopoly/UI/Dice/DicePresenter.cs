using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace JoyMonopoly.UI.Dice
{
    public class DicePresenter : MonoBehaviour, IDice
    {
        [SerializeField] private GameObject[] _sides;
        [SerializeField] private float _changingOneSideToAnotherTime;
        [SerializeField] private int _maxCountSideChanging;

        private GameObject _activeSide;
        
        public async Task SetInitialState()
        {
            if (_activeSide != null)
            {
                _activeSide.SetActive(false);
            }
        }

        public async Task SetActiveSide(int index)
        {
            index--;
            int countOfChanging = Random.Range(1, _maxCountSideChanging);

            for (int i = 0; i < countOfChanging; i++)
            {
                int randomSide = Random.Range(0, 5);

                ShowSide(randomSide);
                
                CancellationTokenSource source = new CancellationTokenSource();
                await Task.Delay(TimeSpan.FromSeconds(_changingOneSideToAnotherTime), source.Token);
                source.Cancel();
                source.Dispose();
            }

            ShowSide(index);
        }

        private void ShowSide(int index)
        {
            if(_activeSide!= null) _activeSide.SetActive(false);

            _activeSide = _sides[index];
            _activeSide.SetActive(true);
        }
    }
}
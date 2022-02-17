using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JoyMonopoly.UI.Dice;
using JoyMonopoly.UI.GameBoard;
using JoyMonopoly.UI.InformationPopup;
using JoyMonopoly.UI.PlayerPanel;
using UnityEngine;
using Zenject;

namespace JoyMonopoly.GameLogic
{
    public class GameManager : IInitializable, ITickable
    {
        private IDice _dice;
        private IGameBoard _gameBoard;
        private IInformationPanel _informationPanel;
        private IPlayerPanel _playerPanel;
        private IRewardCalculator _rewardCalculator;
        private GameSettings _gameSettings;
        private PlayersIDs _activePlayer;
        private Dictionary<PlayersIDs, int> _playersPositions;
        private Dictionary<PlayersIDs, int> _playersWallet;
        private Dictionary<int, PlayersIDs> _playersCells;
        private CellType[] _cellTypes;

        private CancellationTokenSource _source = new CancellationTokenSource();
        private CancellationToken _cancellationToken = new CancellationToken(true);

        public GameManager(
            IDice dice, 
            IGameBoard gameBoard,
            IInformationPanel informationPanel,
            IPlayerPanel playerPanel,
            GameSettings gameSettings, 
            IRewardCalculator rewardCalculator)
        {
            _dice = dice;
            _gameBoard = gameBoard;
            _informationPanel = informationPanel;
            _playerPanel = playerPanel;
            _gameSettings = gameSettings;
            _rewardCalculator = rewardCalculator;
        }

        public void Initialize()
        {
            SetInitState();
        }

        private async Task SetInitState()
        {
            _activePlayer = PlayersIDs.PlayerOne;
            _playersPositions = new Dictionary<PlayersIDs, int>();
            _playersWallet = new Dictionary<PlayersIDs, int>();
            _playersCells = new Dictionary<int, PlayersIDs>();
            _gameBoard.SetInitialState();
            _cellTypes = _gameSettings.CellTypes;

            foreach (var player in _gameSettings.AllPlayers)
            {
                _playersPositions.Add(player, 0);
                _playersWallet.Add(player, _gameSettings.InitPlayerMoney);
                
                if(player == PlayersIDs.PlayerOne)
                    _playerPanel.SetPlayerAsActive(PlayersIDs.PlayerOne, true);
                else
                    _playerPanel.SetPlayerAsActive(player, false);

                _playerPanel.SetPlayerMoney(player, _gameSettings.InitPlayerMoney);
            }

            await _dice.SetInitialState();
            await _informationPanel.Hide();
        }

        public async void Tick()
        {
            if (Input.GetKeyUp(KeyCode.Space) && _cancellationToken.IsCancellationRequested)
            {
                _source = new CancellationTokenSource();
                _cancellationToken = _source.Token;
                await NextGameStep();
                _source.Cancel();
            }

            if (IsOneLost() && _cancellationToken.IsCancellationRequested)
            {
                _source = new CancellationTokenSource();
                _cancellationToken = _source.Token;
                var lostPlayer = _playersWallet.First(x => x.Value <= 0).Key;
                await _informationPanel.ShowMessages(
                    $"Player {(int)lostPlayer}'s",
                    $"Lost"
                );
                _source.Cancel();
                SetInitState();
            }
        }

        private async Task NextGameStep()
        {
            SetNextPlayer();

            int diceValue = RollDice();
            await _dice.SetActiveSide(diceValue);
            int newPlayerPosition = GetPlayerPosition(_activePlayer, diceValue);
            _playersPositions[_activePlayer] = newPlayerPosition;
            await _gameBoard.SetPlayerToCell(_activePlayer, newPlayerPosition);

            if (_cellTypes[newPlayerPosition] == CellType.Start
                || _cellTypes[newPlayerPosition] == CellType.RewardCell)
            {
                var reward = _rewardCalculator.GetReward(_cellTypes[newPlayerPosition]);
                _playersWallet[_activePlayer] += reward;
                _playerPanel.SetPlayerMoney(_activePlayer, _playersWallet[_activePlayer]);
                await _informationPanel.ShowMessages(
                    $"You landed on reward property",
                    $"Get {reward} $"
                );
            }
            
            if (_cellTypes[newPlayerPosition] == CellType.Free 
                && _playersWallet[_activePlayer] > _gameSettings.CellCost)
            {
                _playersWallet[_activePlayer] -= _gameSettings.CellCost;
                _playerPanel.SetPlayerMoney(_activePlayer, _playersWallet[_activePlayer]);
                _cellTypes[newPlayerPosition] = CellType.PlayerCell;
                _playersCells.Add(newPlayerPosition, _activePlayer);
                _gameBoard.SetAsPlayerCell(_activePlayer, newPlayerPosition);
                await _informationPanel.ShowMessages(
                    $"You landed on free property",
                    $"Pay {_gameSettings.CellCost} $"
                );
            }

            if (_cellTypes[newPlayerPosition] == CellType.PlayerCell 
                && _playersCells.ContainsKey(newPlayerPosition) 
                && _playersCells[newPlayerPosition] != _activePlayer)
            {
                var awardedPlayer = _playersCells[newPlayerPosition];
                _playersWallet[_activePlayer] -= _gameSettings.CellCost;
                _playersWallet[awardedPlayer] += _gameSettings.CellCost;
                _playerPanel.SetPlayerMoney(_activePlayer, _playersWallet[_activePlayer]);
                _playerPanel.SetPlayerMoney(awardedPlayer, _playersWallet[awardedPlayer]);
                await _informationPanel.ShowMessages(
                    $"You landed on Player {(int)_activePlayer}'s property",
                    $"Lose {_gameSettings.CellCost} $"
                );
            }
        }

        private void SetNextPlayer()
        {
            if (_activePlayer == PlayersIDs.PlayerOne)
            {
                _activePlayer = PlayersIDs.PlayerTwo;
                _playerPanel.SetPlayerAsActive(PlayersIDs.PlayerTwo, true);
                _playerPanel.SetPlayerAsActive(PlayersIDs.PlayerOne, false);
            }
            else
            {
                _activePlayer = PlayersIDs.PlayerOne;
                _playerPanel.SetPlayerAsActive(PlayersIDs.PlayerTwo, false);
                _playerPanel.SetPlayerAsActive(PlayersIDs.PlayerOne, true);
            }
        }

        private int RollDice()
        {
            return Random.Range(1, 7);
        }

        private int GetPlayerPosition(PlayersIDs playersID, int diceValue)
        {
            int newPlayerPosition = _playersPositions[playersID] + diceValue;

            if (newPlayerPosition >= _gameSettings.CellCount)
                newPlayerPosition = newPlayerPosition - _gameSettings.CellCount;

            return newPlayerPosition;
        }

        private bool IsOneLost()
        {
            return _playersWallet.Any(x => x.Value <= 0);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JoyMonopoly.GameLogic;
using UnityEngine;

namespace JoyMonopoly.UI.GameBoard
{
    public class GameBoardPresenter : MonoBehaviour, IGameBoard
    {
        [SerializeField] private PlayerIdTransforms[] _playersTransforms;
        [SerializeField] private PlayerIdTransforms[] _playerCellsPrefabs;
        
        [SerializeField] private Transform _cellsParent;
        
        [SerializeField] private Vector3[] _cellsPositions;
        [SerializeField] private Quaternion[] _rotations;
        [SerializeField] private int[] _angelCells;
        [SerializeField] private float _playerSpeed;
        
        private Dictionary<PlayersIDs, RectTransform> _playersTransformsDictionary;
        private Dictionary<PlayersIDs, RectTransform> _playerCellsPrefabsDictionary;
        private Dictionary<PlayersIDs, int> _playersCurrentCell;
        private PlayersIDs[] _playersIDs;
        private int _cellsCount;
        
        private void Awake()
        {
            _playersTransformsDictionary = _playersTransforms
                .ToDictionary(x => x.PlayersID, y => y.Transform);
            _playerCellsPrefabsDictionary = _playerCellsPrefabs
                .ToDictionary(x => x.PlayersID, y => y.Transform);

            _playersIDs = _playersTransforms.Select(x => x.PlayersID).ToArray();
            _cellsCount = _cellsPositions.Length;
            
            SetInitialState();
        }

        public void SetInitialState()
        {
            _playersCurrentCell = _playersTransforms
                .ToDictionary(x => x.PlayersID, y => 0);

            foreach (var player in _playersTransformsDictionary)
            {
                player.Value.anchoredPosition = _cellsPositions[0];
            }
            
            foreach (Transform child in _cellsParent)
            {
                GameObject.Destroy(child.gameObject);
            }
        }

        public async Task SetPlayerToCell(PlayersIDs playerID, int targetCell) //used Task because in this method can have animation 
        {
            if(_playersIDs.Contains(playerID))
            {
                int currentCell = _playersCurrentCell[playerID];
                var player = _playersTransformsDictionary[playerID];
                player.SetAsLastSibling();
                
                foreach (var angelCell in _angelCells)
                {
                    if (IsBetween(angelCell, currentCell, targetCell))
                    {
                        await MovePlayer(player, angelCell);
                        await MovePlayer(player, targetCell);
                        _playersCurrentCell[playerID] = targetCell;
                        return;
                    }
                }
                
                await MovePlayer(player, targetCell);
                _playersCurrentCell[playerID] = targetCell;
            }
        }

        public void SetAsPlayerCell(PlayersIDs playersID, int cellNumber)
        {
            var cell = GameObject.Instantiate(_playerCellsPrefabsDictionary[playersID].gameObject).GetComponent<RectTransform>();
            cell.SetParent(_cellsParent);
            cell.anchoredPosition = _cellsPositions[cellNumber];
            cell.GetChild(0).rotation = _rotations[cellNumber];
        }

        private async Task MovePlayer(RectTransform player, int targetCell)
        {
            Vector3 initPlayerPosition = player.anchoredPosition;
            float startTime = Time.time;
            float journeyLength = Vector3.Distance(initPlayerPosition, _cellsPositions[targetCell]);
            
            while (Vector3.Distance(_cellsPositions[targetCell], player.anchoredPosition) > 1)
            {
                float distCovered = (Time.time - startTime) * _playerSpeed;
                float fractionOfJourney = distCovered / journeyLength;
                player.anchoredPosition = Vector3.Lerp(initPlayerPosition,_cellsPositions[targetCell],fractionOfJourney);
                await Task.Yield();
            }
            
            player.anchoredPosition = _cellsPositions[targetCell];
        }

        private bool IsBetween(int number, int start, int end)
        {
            if (start > end)
            {
                return IsBetween(number, start - _cellsPositions.Length, end);
            }

            return number > start && number < end;
        }

        [Serializable]
        public struct PlayerIdTransforms
        {
            public PlayersIDs PlayersID;
            public RectTransform Transform;
        }
    }
}
using System;
using System.Linq;
using UnityEngine;
using Zenject;

namespace JoyMonopoly.GameLogic
{
    [CreateAssetMenu(fileName = "GameSettingsInstaller", menuName = "GameSettings")]
    public class GameSettingsInstaller : ScriptableObjectInstaller<GameSettingsInstaller>
    {
        public GameSettings gameSettings;

        public override void InstallBindings()
        {
            Container.BindInstance(gameSettings).IfNotBound();
        }
    }
    
    [Serializable]
    public class GameSettings
    {
        [SerializeField] private PlayersIDs[] _allPlayers;
        [SerializeField] private int _cellCount;
        [SerializeField] private int _initPlayerMoney;
        [SerializeField] private int _baseReward;
        [SerializeField] private int _cellCost;
        [SerializeField] private RewardSettings[] _rewardSettings;
        [SerializeField] private CellType[] _cellTypes;

        public int InitPlayerMoney => _initPlayerMoney;
        public RewardSettings[] RewardSettings => _rewardSettings;
        public CellType[] CellTypes => _cellTypes.Clone() as CellType[];
        public int CellCount => _cellCount;
        public PlayersIDs[] AllPlayers => _allPlayers;
        public int BaseReward => _baseReward;
        public int CellCost => _cellCost;
    }

}
using System.Linq;
using UnityEngine;

namespace JoyMonopoly.GameLogic
{
    public interface IRewardCalculator
    {
        int GetReward(CellType cellType);
    }

    public class RewardCalculator : IRewardCalculator
    {
        private Reward[] _rewards;
        private int _baseReward;

        public RewardCalculator(GameSettings gameSettings)
        {
            _rewards = new Reward[gameSettings.RewardSettings.Length];

            var rewardSettings = gameSettings.RewardSettings.OrderBy(x => x.Probability).ToArray();
            float previousProbability = 0;
            
            for (int i = 0; i < _rewards.Length; i++)
            {
                var rewardProbability = rewardSettings[i].Probability;
                _rewards[i] = new Reward(previousProbability, rewardProbability, rewardSettings[i].Reward);
                previousProbability = rewardProbability;
            }

            _baseReward = gameSettings.BaseReward;
        }

        public int GetReward(CellType cellType)
        {
            if (cellType == CellType.Start)
                return _baseReward;

            if (cellType == CellType.RewardCell)
                return GetReward();

            return 0;
        }

        private int GetReward()
        {
            float value = Random.Range(0, 1);

            foreach (var reward in _rewards)
            {
                if (reward.InRange(value))
                    return reward.Value;
            }

            return 0;   
        }

        private struct Reward
        {
            public float MinValue;
            public float MaxValue;
            public int Value;

            public Reward(float minValue, float maxValue, int value)
            {
                MinValue = minValue;
                MaxValue = maxValue;
                Value = value;
            }

            public bool InRange(float value)
            {
                return value >= MinValue && value <= MaxValue;
            }
        } 
    }
}
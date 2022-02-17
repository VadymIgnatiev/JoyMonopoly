using JoyMonopoly.GameLogic;
using JoyMonopoly.UI.Dice;
using JoyMonopoly.UI.GameBoard;
using JoyMonopoly.UI.InformationPopup;
using JoyMonopoly.UI.PlayerPanel;
using UnityEngine;
using Zenject;

namespace GameSettings
{
    public class ProjectInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<GameManager>().AsSingle();
            Container.Bind<IRewardCalculator>().To<RewardCalculator>().AsSingle();
            
            Container.Bind<IDice>().To<DicePresenter>().FromComponentsInHierarchy().AsSingle();
            Container.Bind<IGameBoard>().To<GameBoardPresenter>().FromComponentsInHierarchy().AsSingle();
            Container.Bind<IInformationPanel>().To<InformationPopup>().FromComponentsInHierarchy().AsSingle();
            Container.Bind<IPlayerPanel>().To<PlayersPanelPresenter>().FromComponentsInHierarchy().AsSingle();
        }
    }
}

using System.Threading.Tasks;

namespace JoyMonopoly.UI.Dice
{
    public interface IDice
    {
        Task SetInitialState();

        Task SetActiveSide(int index);
    }
}
using System.Threading.Tasks;

namespace JoyMonopoly.UI.InformationPopup
{
    public interface IInformationPanel
    {
        Task ShowMessages(string informationMessage, string attentionMessage);
        Task Hide();
    }
}


namespace SeleneGame.Core.UI {

    public interface IUIMenu : IUI {
        void OnCancel();
        void Refresh();
        void ResetGamePadSelection();
    }

}
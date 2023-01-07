

namespace SeleneGame.Core.UI {

    public interface IUIMenu : IUI {
        void Toggle();
        void OnCancel();
        void ResetGamePadSelection();
    }

}
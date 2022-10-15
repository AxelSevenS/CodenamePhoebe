

namespace SeleneGame.UI {

    public interface IUIMenu : IUI {
        void Toggle();
        void OnCancel();
        void ResetGamePadSelection();
    }

}
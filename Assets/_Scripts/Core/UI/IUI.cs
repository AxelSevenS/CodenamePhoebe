

namespace SeleneGame.Core.UI {

    public interface IUI {
        bool Enabled { get; }
        void Enable();
        void Disable();
        void Toggle();
    }
    
}
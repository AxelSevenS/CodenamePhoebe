using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;

using SeleneGame.Core;

using SevenGame.Utility;
using System.Threading.Tasks;

namespace SeleneGame.Core.UI {
    
    public abstract class UIMenu<T> : UI<T>, IUIMenu where T : UIMenu<T> {

        
        public virtual void Toggle() {
            if (Enabled)
                Disable();
            else
                Enable();
        }

        public override void Enable() {

            UIController.current.onGaySex += OnCancel;

            UIController.currentMenu?.Disable();
            UIController.currentMenu = this;
            

            UIController.current.UpdateMenuState();

            base.Enable();

        }

        public override void Disable() {

            UIController.current.onGaySex -= OnCancel;

            if ( (Object)UIController.currentMenu == this)
                UIController.currentMenu = null;
            
            UIController.current.UpdateMenuState();

            base.Disable();
        }

        public virtual void OnCancel() {
            Disable();
        }

        public virtual void OnControllerTypeChange(Keybinds.ControllerType type) {
            ResetGamePadSelection();
        }

        public abstract void ResetGamePadSelection();

    }

}
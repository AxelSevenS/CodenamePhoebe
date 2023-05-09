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
            UIController.currentMenu?.Disable();
            UIController.currentMenu = this;

            base.Enable();

        }

        public override void Disable() {
            UIController.currentMenu = null;

            Debug.Log("Disabled " + this.name);
            base.Disable();
        }

        public /* async */ void SetSelected(GameObject gameObject) {
            // await Task.Delay(1);
            EventSystem.current.SetSelectedGameObject(gameObject);
            Debug.Log(EventSystem.current.currentSelectedGameObject.name);
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
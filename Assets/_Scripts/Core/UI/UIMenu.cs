using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;

using SeleneGame.Core;

using SevenGame.Utility;
using System.Threading.Tasks;

namespace SeleneGame.Core.UI {
    
    public abstract class UIMenu<T> : UI<T>, IUIMenu where T : UIMenu<T> {

        public virtual void OnCancel() {
            Disable();
        }

        public virtual void OnControllerTypeChange(Keybinds.ControllerType type) {
            ResetGamePadSelection();
        }

        public abstract void Refresh();

        public abstract void ResetGamePadSelection();


        protected override void OnEnable() {
            base.OnEnable();
            // UIController.current.onCancel += OnCancel;
        }


        protected override void OnDisable() {
            base.OnDisable();
            // UIController.current.onCancel -= OnCancel;
        }
    }

}
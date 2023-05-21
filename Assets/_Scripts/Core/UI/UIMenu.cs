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


        public override void Enable() {

            if ( UIController.modalRoot == null ) {
                UIController.modalRoot = this;
            }

            transform.SetSiblingIndex( UIController.current.transform.childCount - 1 );

            base.Enable();
        }


        public override void Disable() {

            if ( UIController.modalRoot == (IUIModal)this ) {
                UIController.DisableModalTree();
                UIController.modalRoot = null;
            }
            
            base.Disable();
        }

        public abstract void EnableInteraction();

        public abstract void DisableInteraction();
    }

}
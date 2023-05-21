using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core.UI {

    public abstract class UIModal<T> : UIMenu<T>, IUIModal where T : UIMenu<T> {

        protected IUIMenu _previousModal;
        protected IUIMenu _nextModal;

        public IUIMenu previousModal => _previousModal;
        public IUIMenu nextModal => _nextModal;

        public override void Enable() {

            if ( UIController.modalLeaf != (IUIModal)this) {
                _previousModal = UIController.modalLeaf;
                UIController.modalLeaf?.DisableInteraction();
                UIController.modalLeaf = this;
            }

            base.Enable();
        }


        public override void Disable() {


            if ( UIController.modalLeaf == (IUIModal)this) {
                UIController.modalLeaf = _previousModal;
            }
            _previousModal?.Refresh();
            _previousModal?.EnableInteraction();
            _previousModal = null;
            

            base.Disable();
        }

    }
}

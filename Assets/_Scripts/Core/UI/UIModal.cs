using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core.UI {

    public abstract class UIModal<T> : UIMenu<T>, IUIModal where T : UIMenu<T> {

        protected IUIMenu _previousModal;

        public IUIMenu previousModal => _previousModal;

        public override void Enable() {

            _previousModal = UIController.modalLeaf;
            UIController.modalLeaf = this;

            base.Enable();
        }


        public override void Disable() {

            _previousModal?.Refresh();
            UIController.modalLeaf = _previousModal;
            _previousModal = null;

            base.Disable();
        }

    }
}
